using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wissance.WebApiToolkit.Tests.TestServer
{
    internal class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            // Configuration = configuration;
            // Environment = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // ConfigureLogging(services);
            // ConfigureDatabase(services);
            // ConfigureWebApi(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}