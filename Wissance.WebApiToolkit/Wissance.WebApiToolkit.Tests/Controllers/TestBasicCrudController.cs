using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.Tests.Utils;
using Wissance.WebApiToolkit.Tests.Utils.Checkers;

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
                Assert.NotNull(result);
                Assert.True(result.Success);
                // todo(UMV): perform body check
            }
        }
        
        [Theory]
        [InlineData(1)]
        public async Task TestUpdateAsync(int organizationId)
        {
            using (HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync($"api/Organization/{organizationId}");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string orgDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(orgDataStr.Length > 0);
                OperationResultDto<OrganizationDto> result = JsonConvert.DeserializeObject<OperationResultDto<OrganizationDto>>(orgDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                result.Data.Name = "TestNameUpd LLC";
                result.Data.ShortName = "TestNameUpd";
                JsonContent content = JsonContent.Create(result.Data);
                resp = await client.PutAsync($"api/Organization/{organizationId}", content);
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string updOrgDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(updOrgDataStr.Length > 0);
                OperationResultDto<OrganizationDto> updResult = JsonConvert.DeserializeObject<OperationResultDto<OrganizationDto>>(updOrgDataStr);
                Assert.NotNull(updResult);
                Assert.True(updResult.Success);
                OrganizationChecker.Check(result.Data, updResult.Data);
            }
        }
        
        [Fact]
        public async Task TestDeleteAsync()
        {
            // todo(UMV):implement
        }
    }
}