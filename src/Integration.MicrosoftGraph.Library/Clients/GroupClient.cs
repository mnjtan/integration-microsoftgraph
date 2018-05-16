using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Integration.MicrosoftGraph.Library.Models;
using System.Collections.Generic;

namespace Integration.MicrosoftGraph.Library.Clients
{
    public class GroupClient
    {
        private string clientId { get; set; }
        private string clientSecret { get; set; }
        private string tenant { get; set; }
        private AuthenticationContext authContext;
        private ClientCredential credential;
        private const string msGraphScope = "https://graph.microsoft.com/.default";
        private const string msGraphQuery = "https://graph.microsoft.com/v1.0";

        public GroupClient(string cid, string cs, string t)
        {
            this.clientId = cid;
            this.clientSecret = cs;
            this.tenant = t;
            this.authContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            
            this.credential = new ClientCredential(clientId, clientSecret);
        }

        public async Task<List<string>> GetAllGroups()
        {
            var groupsString = await SendGraphGetRequest("/groups", "");
            var groupResponse = JsonConvert.DeserializeObject<MSGraphGroupsListResponse>(groupsString);

            List<string> groupList =  new List<string>();
            foreach(var x in groupResponse.value)
            {
                groupList.Add(x.displayName);
            }

            return groupList;
        }

        public async Task<string> GetGroupById(string groupId)
        {
            var groupString = await GetGroups(groupId);
            var singleGroup = JsonConvert.DeserializeObject<GetGroupModel>(groupString);

            return singleGroup.displayName;
        }

        public async Task<string> GetGroupByName(string groupName)
        {
            var groupsString = await SendGraphGetRequest("/groups", "");
            var groupResponse = JsonConvert.DeserializeObject<MSGraphGroupsListResponse>(groupsString);

            string realId = "No Id Found";
            foreach (var x in groupResponse.value)
            {
                if (x.displayName == groupName)
                {
                    realId = x.id;
                    return realId;
                }
            }

            return realId;
        }

        public async Task<string> CreateGroup(GroupModel group)
        {
            //Needs id, displayName, mailEnabled, mailNickname
            var groupJson = JsonConvert.SerializeObject(group);
            return await SendGraphPostRequest("/groups", groupJson);
        }


        public async Task<string> AddUserToGroup(string groupId, string userId)
        {
            UserToGroup utg = new UserToGroup(userId);
            var groupJson = JsonConvert.SerializeObject(utg);
            string fullRequest = "/groups/" + groupId + "/members/$ref";
            return await SendGraphPostRequest(fullRequest, groupJson);
        }

        public async Task<string> GetGroups(string query)
        {
            return await SendGraphGetRequest("/groups", query);
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
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", credential);

            HttpClient http = new HttpClient();

            var url = msGraphQuery + api;

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

            return await response.Content.ReadAsStringAsync();
        }
    }
}
