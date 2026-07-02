using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.Tests.Utils;
using Wissance.WebApiToolkit.Tests.Utils.Operations;

namespace Wissance.WebApiToolkit.Tests.Controllers
{
    public class TestConfiguredController : WebApiTestBasedOnTestApp
    {
        [Fact]
        public async Task TestReadSuccessfullyAsync()
        {
            using (HttpClient client = Application.CreateClient())
            {
                // TODO(UMV): check data in future
                OperationResultDto<PagedDataDto<ProfileDto>> result = await TestBasicHttpInteraction.ExecReadManyAndCheckAsync<ProfileDto>(client, "api/Profile", HttpStatusCode.OK);
                Assert.NotNull(result);
            }
        }
        
        [Fact]
        public async Task TestCreateSuccessfullyAsync()
        {
            
        }
        
        [Fact]
        public async Task TestUpdateSuccessfulyAsync()
        {
            
        }
        
        [Fact]
        public async Task TestDeleteFailsAsync()
        {
            
        }
    }
}