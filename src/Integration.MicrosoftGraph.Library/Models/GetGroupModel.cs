using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Integration.MicrosoftGraph.Library.Models
{
    public class GetGroupModel
    {
        public string id {get; set;}
        public string description {get; set;}
        public string displayName {get; set;}
        public List<string> groupTypes {get; set;}
        public bool mailEnabled {get; set;}
        public string mailNickname {get; set;}
        public string onPremisesLastSyncDatetime {get; set;}
        public string onPremisesSecurityIdentifier {get; set;}
        //public bool onPremisesSyncEnabled {get; set;}
        public List<string> proxyAddresses {get; set;}
        public bool securityEnabled {get; set;}
        public string visibility {get; set;}
    }
}