
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Integration.MicrosoftGraph.Library.Clients;
using Integration.MicrosoftGraph.Service.Models;
using Integration.MicrosoftGraph.Library.Models;
using System;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Integration.MicrosoftGraph.Service.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SyncUsersController : Controller
    {

        private MSGraphClient client { set; get; }
        private string sfEndPoint { set; get; }

        public SyncUsersController(ReadAppSettings settings)
        {
            sfEndPoint = settings.salesforce_endpoint;
            var tenant = settings.microsoft_tenant;
            var clientId = settings.microsoft_client_id;
            var clientSecret = settings.microsoft_client_secret;
            client = new MSGraphClient(clientId, clientSecret, tenant);
        }

        [HttpGet]
        public async Task GetUser()
        {   
            //Get SalesForce Users
            var httpclient = new HttpClient();
            var SFResult = await httpclient.GetAsync(sfEndPoint);
            var SFUsers = JsonConvert.DeserializeObject<List<SalesforceUser>>(await SFResult.Content.ReadAsStringAsync());

            //Get Azure AD Users
            var msusers = await client.GetUsers("");
            var ADUsersResponse = JsonConvert.DeserializeObject<MSGraphUserListResponse>(msusers);
            var ADUsers = ADUsersResponse.value;

            var ADUsersToDelete = new List<User>();
            var SFUsersToDelete = new List<SalesforceUser>();

            foreach(var adUser in ADUsers)
            {
                foreach(var sfUser in SFUsers)
                {
                    if (sfUser.EMail == adUser.mail)
                    {
                        SFUsersToDelete.Add(sfUser);
                        ADUsersToDelete.Add(adUser);
                    }
                }
            }

            foreach (var adUser in ADUsersToDelete)
            {
                ADUsers.Remove(adUser);
            }
            foreach (var sfUser in SFUsersToDelete)
            {
                SFUsers.Remove(sfUser);
            }
        

            foreach (var sfUser in SFUsers)
            {
                var inviteResponse = await client.InviteUser(sfUser);
                var invitation = JsonConvert.DeserializeObject<Invitation>(inviteResponse);
                var uid = invitation.invitedUser.id;
                Console.WriteLine(inviteResponse);
            }
        }
    }
}