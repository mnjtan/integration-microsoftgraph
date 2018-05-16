using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Integration.MicrosoftGraph.Service
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
      Configuration = configuration;
      Env = env;
    }

    public IHostingEnvironment Env {get; }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
            List<string> strings = new List<string>();
            if(Env.IsStaging())
            {
                strings.Add(Environment.GetEnvironmentVariable("MICROSOFT_GRAPH_CLIENT_ID"));
                strings.Add(Environment.GetEnvironmentVariable("MICROSOFT_GRAPH_CLIENT_SECRET"));
                strings.Add(Environment.GetEnvironmentVariable("MICROSOFT_GRAPH_TENANT"));
                strings.Add(Environment.GetEnvironmentVariable("SALESFORCE_ENDPOINT"));
            }
            else
            {
                strings.Add(Configuration.GetSection("tenant").ToString());
                strings.Add(Configuration.GetSection("clientId").ToString());
                strings.Add(Configuration.GetSection("clientSecret").ToString());
                strings.Add(Configuration.GetSection("salesforceEndPoint").ToString());

            }
            ReadAppSettings settings = new ReadAppSettings(strings);
            services.AddSingleton(settings);
            
      services.AddMvc();

      //Register ther Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen( c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddApplicationInsights(app.ApplicationServices);

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Integration.MicrosoftGraph.Service API v1");
      });
       
      app.UseMvc();
    }
  }
}
