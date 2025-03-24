using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.Tests.Utils;

namespace Wissance.WebApiToolkit.Tests.Controllers
{
    public class TestBasicCrudController : WebApiTestBasedOnTestApp
    {
        [Fact]
        public async Task TestReadAsync()
        {
            using (HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("api/Organization");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<OrganizationDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<OrganizationDto>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestCreateAsync()
        {
            using (HttpClient client = Application.CreateClient())
            {
                OrganizationDto organization = new OrganizationDto()
                {
                    Name = "LLC SuperDuper",
                    ShortName = "SuperDuper",
                    TaxNumber = "999091234",
                    Codes = new List<int>(){1, 3}
                };
                JsonContent content = JsonContent.Create(organization);
                HttpResponseMessage resp = await client.PostAsync("api/Organization", content);
                Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
                string orgCreateDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(orgCreateDataStr.Length > 0);
                OperationResultDto<OrganizationDto> result = JsonConvert.DeserializeObject<OperationResultDto<OrganizationDto>>(orgCreateDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
            }
        }
        
        [Fact]
        public async Task TestUpdateAsync()
        {
        }
        
        [Fact]
        public async Task TestDleteAsync()
        {
        }
    }
}