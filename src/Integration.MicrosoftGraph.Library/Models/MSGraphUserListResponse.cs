using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Integration.MicrosoftGraph.Library.Models
{
    public class MSGraphUserListResponse
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string context { get; set; }
        
        [JsonProperty(PropertyName = "@odata.nextLink")]
        public string nextLink { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<User> value { get; set; }

        public override string ToString()
        {
            string returnString = $"MSGraphUserListResponse {{odata.context:{context}\nodata.nextLink:{nextLink}}} ";
            foreach(var user in value)
            {
                returnString += "\n" + user;
            }
            return returnString;
        }
    }

}