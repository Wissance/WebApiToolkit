using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.Tests.Utils;

namespace Wissance.WebApiToolkit.Tests.Controllers
{
    public class TestCrudControllerWithDynamicAutoManager : WebApiTestBasedOnTestApp
    {
        [Fact]
        public async Task TestReadAsync()
        {
            using (HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("api/User");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<UserEntity>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<UserEntity>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
            }
        }
    }
}