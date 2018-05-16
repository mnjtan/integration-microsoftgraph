
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Integration.MicrosoftGraph.Library.Models
{
    public class User
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string jobTitle { get; set; }
        public string mail { get; set; }
        public string mobilePhone { get; set; }
        public string officeLocation { get; set; }
        public string preferredLanguage { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public bool accountEnabled = true;
        public string mailNickname { get; set; }
        public PasswordProfile passwordProfile { get; set; }



        public override string ToString()
        {
            string returnString = $"User {{id:{id}}}";
            returnString += $"\n\tdisplayName:{displayName}\n\tgivenName:{givenName}\n\tjobTitle:{jobTitle}";
            returnString += $"\n\tmail:{mail}\n\tmobilePhone:{mobilePhone}\n\tofficeLocation:{officeLocation}";
            returnString += $"\n\tpreferredLanguage:{preferredLanguage}\n\tsurname:{surname}\n\tuserPrincipalName:{userPrincipalName} }} ";
            return returnString;
        }

    }
    
    public class PasswordProfile
    {
        public bool forceChangePasswordNextSignIn { get; set; }
        public string password { get; set; }

        public PasswordProfile(string password, bool forceChangePasswordNextSignIn = true)
        {
            this.password = password;
            this.forceChangePasswordNextSignIn = forceChangePasswordNextSignIn;
        }
    }

}