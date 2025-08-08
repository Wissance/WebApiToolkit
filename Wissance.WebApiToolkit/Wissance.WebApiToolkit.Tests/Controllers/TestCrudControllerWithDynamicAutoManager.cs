using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.Tests.Utils;
using Wissance.WebApiToolkit.Tests.Utils.Checkers;

namespace Wissance.WebApiToolkit.Tests.Controllers
{
    public class TestCrudControllerWithDynamicAutoManager : WebApiTestBasedOnTestApp
    {
        [Fact]
        public async Task TestReadMany()
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

        [Fact]
        public async Task TestCreate()
        {
            using (HttpClient client = Application.CreateClient())
            {
                UserEntity newUser = new UserEntity()
                {
                    Id = 100,
                    Login = "ass",
                    FullName = "Ivan Ivanov",
                    OrganizationId = 4
                };
                JsonContent content = JsonContent.Create(newUser);
                HttpResponseMessage createUserResponse = await client.PostAsync("api/User", content);
                Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
                string userCreateDataStr = await createUserResponse.Content.ReadAsStringAsync();
                Assert.True(userCreateDataStr.Length > 0);
                OperationResultDto<UserEntity> result = JsonConvert.DeserializeObject<OperationResultDto<UserEntity>>(userCreateDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                UserChecker.Check(newUser, result.Data);
            }
        }

        [Fact]
        public async Task TestUpdate()
        {
            using (HttpClient client = Application.CreateClient())
            {
                UserEntity newUser = new UserEntity()
                {
                    Id = 100,
                    Login = "ass",
                    FullName = "Ivan Ivanov",
                    OrganizationId = 4
                };
                JsonContent content = JsonContent.Create(newUser);
                HttpResponseMessage createUserResponse = await client.PostAsync("api/User", content);
                Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
                string userCreateDataStr = await createUserResponse.Content.ReadAsStringAsync();
                Assert.True(userCreateDataStr.Length > 0);
                OperationResultDto<UserEntity> result = JsonConvert.DeserializeObject<OperationResultDto<UserEntity>>(userCreateDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                UserChecker.Check(newUser, result.Data);
                newUser.Login = "paa";
                newUser.OrganizationId = 8;
                newUser.FullName = "Petr Petrov";
                content = JsonContent.Create(newUser);
                HttpResponseMessage updateUserResponse = await client.PutAsync($"api/User/{newUser.Id}", content);
                Assert.Equal(HttpStatusCode.OK, updateUserResponse.StatusCode);
                string userUpdateDataStr = await createUserResponse.Content.ReadAsStringAsync();
                Assert.True(userUpdateDataStr.Length > 0);
                result = JsonConvert.DeserializeObject<OperationResultDto<UserEntity>>(userCreateDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                UserChecker.Check(newUser, result.Data);
            }
        }
        
        [Fact]
        public async Task TestDelete()
        {
            // todo(UMV):implement ...
        }
    }
} 