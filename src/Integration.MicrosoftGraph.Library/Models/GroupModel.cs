using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Integration.MicrosoftGraph.Library.Models
{
    public class GroupModel
    {
        public string description {get; set;}
        public string displayName {get; set;}
        public bool mailEnabled {get; set;}
        public string mailNickname {get; set;}
        public bool securityEnabled {get; set;}
    }

    public class MSGraphGroupsListResponse
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string context { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<GetGroupModel> value { get; set; }
    }
}
