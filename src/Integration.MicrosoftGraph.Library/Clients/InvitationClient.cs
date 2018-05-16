using Integration.MicrosoftGraph.Library.Models;
using Integration.MicrosoftGraph.Service.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Integration.MicrosoftGraph.Library.Clients
{
    public class InvitationClient
    {
        private string clientId { get; set; }
        private string clientSecret { get; set; }
        private string tenant { get; set; }
        private AuthenticationContext authContext;
        private ClientCredential credential;
        private const string msGraphScope = "https://graph.microsoft.com/.default";
        private const string msGraphQuery = "https://graph.microsoft.com/v1.0";

        public InvitationClient(string clientId, string clientSecret, string tenant)
        {
            // The client_id, client_secret, and tenant are pulled in from the App.config file
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.tenant = tenant;

            // The AuthenticationContext is ADAL's primary class, in which you indicate the direcotry to use.
            this.authContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);

            // The ClientCredential is where you pass in your client_id and client_secret, which are 
            // provided to Azure AD in order to receive an access_token using the app's identity.
            this.credential = new ClientCredential(clientId, clientSecret);
        }


        
        public async Task<string> InviteUser(SalesforceUser SFUser)
        {
            Invitation invitation = new Invitation();
            invitation.invitedUserEmailAddress = SFUser.EMail;
            invitation.inviteRedirectUrl = "Revature_housing_homepage";
            invitation.sendInvitationMessage = true;
            var json = JsonConvert.SerializeObject(invitation);
            return await PostInvitation("/invitations", json);
        }
        private async Task<string> PostInvitation(string api, string json)
        {
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", credential);
            HttpClient http = new HttpClient();

            var url = msGraphQuery + api;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue(result.AccessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}