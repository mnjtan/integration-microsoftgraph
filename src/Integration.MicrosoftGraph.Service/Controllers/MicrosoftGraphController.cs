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
        private static string tenant = ReadAppSettings.tenant;
        private static string clientId = ReadAppSettings.clientId;
        private static string clientSecret = ReadAppSettings.clientSecret;
        private static MSGraphClient client = new MSGraphClient(clientId, clientSecret, tenant);

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