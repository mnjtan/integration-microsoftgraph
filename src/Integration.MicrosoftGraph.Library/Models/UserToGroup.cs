using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Integration.MicrosoftGraph.Library.Models
{
    public class UserToGroup
    {
        [JsonProperty(PropertyName = "@odata.id")]
        public string id { get; set; }

        public UserToGroup(string userId)
        {
            id = "https://graph.microsoft.com/v1.0/directoryObjects/" + userId;
        }
    }
}
