using Microsoft.AspNetCore.Mvc.Testing;
using Wissance.WebApiToolkit.TestApp;

namespace Wissance.WebApiToolkit.Tests.Utils
{
    public class WebApiTestBasedOnTestApp
    {
        public WebApiTestBasedOnTestApp()
        {
            Application = new WebApplicationFactory<Startup>();
        }
        
        public WebApplicationFactory<Startup> Application { get; }
    }
}