using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Tests.Utils;

namespace Wissance.WebApiToolkit.Tests.Controllers
{
    public class TestBasicReadController : WebApiTestBasedOnTestApp
    {
        [Fact]
        public async Task TestReadAsync()
        {
            using(HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("/api/Code");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            }
        }
    }
}