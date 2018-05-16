using Integration.MicrosoftGraph.Library.Models;
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
    public class MSGraphClient
    {
        private string clientId { get; set; }
        private string clientSecret { get; set; }
        private string tenant { get; set; }
        private AuthenticationContext authContext;
        private ClientCredential credential;
        private const string msGraphScope = "https://graph.microsoft.com/.default";
        private const string msGraphQuery = "https://graph.microsoft.com/v1.0";

        public MSGraphClient(string clientId, string clientSecret, string tenant)
        {
            // The client_id, client_secret, and tenant are pulled in from the App.config file
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.tenant = tenant;
            //private const string authorityFormat = "https://login.microsoftonline.com/{0}/v2.0";
            // The AuthenticationContext is ADAL's primary class, in which you indicate the direcotry to use.
            this.authContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);

            // The ClientCredential is where you pass in your client_id and client_secret, which are 
            // provided to Azure AD in order to receive an access_token using the app's identity.
            this.credential = new ClientCredential(clientId, clientSecret);
        }

        public async Task<string> GetUsers(string query)
        {
            return await SendGraphGetRequest("/users", query);
        }

        public async Task<string> GetUserId(string email)
        {
            string usersString = await GetUsers("");
            var usersResponse = JsonConvert.DeserializeObject<MSGraphUserListResponse>(usersString);
            var users = usersResponse.value;
            
            foreach(var u in users)
            {
                if(u.mail == email)
                {
                    return u.id;
                }
            }
            return null;
        }

        public async Task<string> CreateUser(string json)
        {
            return await SendGraphPostRequest("/users", json);
        }

        public async Task<string> DeleteUser(string apiExt)
        {
            return await SendGraphDeleteRequest(apiExt);
        }

        public async Task<string> SendGraphGetRequest(string api, string query)
        {

            // First, use ADAL to acquire a token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for; in this case, the Graph API.
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", credential);
            
            HttpClient client = new HttpClient();
            var url = msGraphQuery + api;
            Console.WriteLine("query: {0}", query);
            if (!string.IsNullOrEmpty(query))
            {
                url += "/" + query;
            } 

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("GET " + url);
            Console.WriteLine("Authorization: Bearer " + result.AccessToken.Substring(0, 80) + "...");
            Console.WriteLine("");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);


            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the MS Graph: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> SendGraphPostRequest(string api, string json)
        {
            
            // First, use ADAL to acquire a token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for; in this case, the Graph API.
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", credential);

            HttpClient http = new HttpClient();
            var url = msGraphQuery + api;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("POST " + url);
            Console.WriteLine("Authorization: Bearer " + result.AccessToken.Substring(0, 80) + "...");
            Console.WriteLine("Content-Type: application/json");
            Console.WriteLine("");
            Console.WriteLine(json);
            Console.WriteLine("");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the MS Graph: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine((int)response.StatusCode + ": " + response.ReasonPhrase);
            Console.WriteLine("");

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> SendGraphDeleteRequest(string apiExt)
        {
            // First, use ADAL to acquire a token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for; in this case, the Graph API.
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", credential);

            HttpClient http = new HttpClient();
            var url = msGraphQuery + apiExt;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("DELETE " + url);
            Console.WriteLine("Authorization: Bearer " + result.AccessToken.Substring(0, 80) + "...");
            Console.WriteLine("");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine((int)response.StatusCode + ": " + response.ReasonPhrase);
            Console.WriteLine("");

            return await response.Content.ReadAsStringAsync();
        }
    }
}
 