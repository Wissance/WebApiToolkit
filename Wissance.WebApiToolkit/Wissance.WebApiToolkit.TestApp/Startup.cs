
namespace Wissance.WebApiToolkit.TestApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogging(services);
            ConfigureDatabase(services);
            ConfigureWebApi(services);
        }

        private void ConfigureLogging(IServiceCollection services)
        {
        }
        
        private void ConfigureDatabase(IServiceCollection services)
        {
        }

        private void ConfigureWebApi(IServiceCollection services)
        {
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
    }
}