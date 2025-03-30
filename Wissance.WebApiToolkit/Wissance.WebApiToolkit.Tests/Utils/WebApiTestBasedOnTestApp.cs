using System.Net.Http;
using Grpc.Net.Client;
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

        public GrpcChannel CreateChannel()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost", 
                new GrpcChannelOptions()
                {
                    HttpClient = Application.CreateClient()
                });
            return channel;
        }

        public WebApplicationFactory<Startup> Application { get; }
    }
}