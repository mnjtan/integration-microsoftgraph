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
        public static IConfiguration Configuration { get; set; }

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.dev.json");

        public static string tenant = Configuration["tenant"];
        public static string clientId = Configuration["clientId"];
        public static string clientSecret = Configuration["clientSecret"];
    }
}