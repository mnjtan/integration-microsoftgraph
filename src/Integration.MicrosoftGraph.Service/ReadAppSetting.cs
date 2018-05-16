using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Integration.MicrosoftGraph.Service
{
    public class ReadAppSettings
    {
        public readonly string microsoft_tenant;
        public readonly string microsoft_client_id;
        public readonly string microsoft_client_secret;
        public readonly string salesforce_endpoint;

        public ReadAppSettings(List<string> strings)
        {
            foreach(string s in strings)
            {
                Console.WriteLine("Strings in Configuration: ");
                Console.WriteLine(s);
                Console.WriteLine(s.ToString());
            }

            microsoft_tenant = strings[0];
            microsoft_client_id = strings[1];
            microsoft_client_secret = strings[2];
            salesforce_endpoint = strings[3];
        }

    }
}
