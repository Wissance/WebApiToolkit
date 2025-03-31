using System.Net;
using System.Net.Http;
using System.Text;
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
        
        [Theory]
        [InlineData(0, 0, 1, 1, 3)]
        [InlineData(1, -1, 1, 1, 3)]
        [InlineData(0, null, 1, 1, 3)]
        [InlineData(1, null, 1, 1, 3)]
        [InlineData(-1, -1, 1, 1, 3)]
        [InlineData(null, null, 1, 1, 3)]
        [InlineData(1, 10, 1, 1, 3)]
        [InlineData(2, 2, 2, 2, 3)]
        public async Task TestReadAsync(int? page, int? size, int expectedPage, int expectedPages, int expectedTotal)
        {
            using(HttpClient client = Application.CreateClient())
            {
                StringBuilder resourceUri = new StringBuilder("/api/Code");
                if (page != null)
                    resourceUri.Append($"?page={page.Value}");

                if (size != null)
                {
                    if (page != null)
                        resourceUri.Append($"&size={size.Value}");
                    else
                        resourceUri.Append($"?size={size.Value}");
                }

                HttpResponseMessage resp = await client.GetAsync(resourceUri.ToString());
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<CodeDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<CodeDto>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Equal(expectedPage, result.Data.Page);
                Assert.Equal(expectedPages, result.Data.Pages);
                Assert.Equal(expectedTotal, result.Data.Total);
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
                Assert.NotNull(result);
                Assert.True(result.Success);
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