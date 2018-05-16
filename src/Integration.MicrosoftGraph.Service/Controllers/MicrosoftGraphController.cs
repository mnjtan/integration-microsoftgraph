using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Integration.MicrosoftGraph.Library.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Integration.MicrosoftGraph.Library.Clients;

namespace Integration.MicrosoftGraph.Service.Controllers
{
  [Route("api/[controller]")]
  [Produces("application/json")]
  public class MicrosoftGraphController : Controller
  {
        private string tenant { set; get; }
        private string clientId { set; get; }
        private string clientSecret { set; get; }
        private MSGraphClient client { set; get; }

        public MicrosoftGraphController(ReadAppSettings settings)
        {
            tenant = settings.microsoft_tenant;
            clientId = settings.microsoft_client_id;
            clientSecret = settings.microsoft_client_secret;
            System.Console.WriteLine("Tenant {0}", tenant);
            System.Console.WriteLine("ClientID {0}", clientId);
            System.Console.WriteLine("ClientSecret {0}", clientSecret);
            client = new MSGraphClient(clientId, clientSecret, tenant);
        }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var usersString = await client.GetUsers("");
        var usersResponse = JsonConvert.DeserializeObject<MSGraphUserListResponse>(usersString);
        return await Task.Run(() => Ok(usersResponse.value));
    }

    [HttpGet("{userPrincipalName}")]
    public async Task<IActionResult> Get(string userPrincipalName)
    {
        var userString = await client.GetUsers(userPrincipalName);
        var singleUser = JsonConvert.DeserializeObject<User>(userString);
        return await Task.Run(() => Ok(singleUser));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]User user)
    {
      var userJson = JsonConvert.SerializeObject(user);
      var creationResponse = await client.CreateUser(userJson);
      return await Task.Run(() => Ok(creationResponse));
    }
  }
}