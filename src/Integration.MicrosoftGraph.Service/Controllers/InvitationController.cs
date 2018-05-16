
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
    public class InvitationController : Controller
    {

        private string tenant { set; get; }
        private string clientId { set; get; }
        private string clientSecret { set; get; }
        private string sfEndPoint { set; get; }

        private InvitationClient inviteClient { set; get; }

        public InvitationController(ReadAppSettings settings)
        {
            tenant = settings.microsoft_tenant;
            clientId = settings.microsoft_client_id;
            clientSecret = settings.microsoft_client_secret;
            sfEndPoint = settings.salesforce_endpoint;
            inviteClient = new InvitationClient(clientId, clientSecret, tenant);
        }

        [HttpGet]
        public async Task GetUser()
        {   
            var client = new HttpClient();
            var SFResult = await client.GetAsync(sfEndPoint);
            var SFUsers = JsonConvert.DeserializeObject<List<SalesforceUser>>(await SFResult.Content.ReadAsStringAsync());
            MSGraphClient msclient = new MSGraphClient(clientId, clientSecret, tenant);
            var msusers = await msclient.GetUsers("");
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
            
            foreach (var adUser in ADUsers)
            {
                await msclient.DeleteUser("/users/" + adUser.id);
            }

            foreach (var sfUser in SFUsers)
            {
                
                await inviteClient.InviteUser(sfUser);
                var uid = await msclient.GetUserId(sfUser.EMail);
                // call brandon's add user to group (GetGroupByName("groupName"), userID)
                GroupClient gclient = new GroupClient(clientId, clientSecret, tenant);
                string gid = await gclient.GetGroupByName("Associates");
                if (String.IsNullOrEmpty(gid))
                {
                    var group = new GroupModel();
                    group.description = "Associate Group";
                    group.displayName = "Associate";
                    group.mailEnabled = false;
                    group.mailNickname = "Associate Mail";
                    group.securityEnabled = false;
                    await gclient.CreateGroup(group);

                    gid = await gclient.GetGroupByName("Associates");
                }

                await gclient.AddUserToGroup(gid, uid);
            }
        }
    }
}