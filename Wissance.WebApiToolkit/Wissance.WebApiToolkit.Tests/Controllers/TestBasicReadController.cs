using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.Tests.Utils;
using Wissance.WebApiToolkit.Tests.Utils.Checkers;

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
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<CodeDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<CodeDto>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.Equal(true, result.Success);
                // See DataInitializer, just test data by it size that we've received anything
                Assert.Equal(3, result.Data.Total);
            }
        }

        [Fact]
        public async Task TestReadByIdAsync()
        {
            using(HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("/api/Code/2");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string readStr = await resp.Content.ReadAsStringAsync();
                Assert.True(readStr.Length > 0);
                OperationResultDto<CodeDto> result = JsonConvert.DeserializeObject<OperationResultDto<CodeDto>>(readStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.Equal(true, result.Success);
                CodeDto expectedCode = new CodeDto()
                {
                    Id = 2,
                    Code = "2",
                    Name = "Hardware development"
                };
                CodeChecker.Check(expectedCode, result.Data);
            }
            
        }
    }
}