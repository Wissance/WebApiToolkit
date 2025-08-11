using System;
using System.Collections.Generic;
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
    public class TestBulkCrudControllerWithDynamicAutoManager : WebApiTestBasedOnTestApp
    {
        [Fact]
        public async Task TestReadMany()
        {
            using (HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<RoleEntity>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleEntity>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
            }
        }
        
        [Fact]
        public async Task TestBulkCreate()
        {
            using (HttpClient client = Application.CreateClient())
            {
                // 1. Getting roles and count them
                HttpResponseMessage resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<RoleEntity>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleEntity>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                int beforeBulkCreate = result.Data.Data.Count;

                // 2. Call bulk create method
                RoleEntity[] roles = new RoleEntity[]
                {
                    new RoleEntity()
                    {
                        Name = "admin",
                        UserId = 3,
                    },
                    new RoleEntity()
                    {
                        Name = "tech",
                        UserId = 3,
                    },
                    new RoleEntity()
                    {
                        Name = "devops",
                        UserId = 3,
                    },
                    new RoleEntity()
                    {
                        Name = "admin",
                        UserId = 5,
                    },
                    new RoleEntity()
                    {
                        Name = "sys",
                        UserId = 5,
                    }
                };
                
                JsonContent content = JsonContent.Create(roles);
                HttpResponseMessage createUserResponse = await client.PostAsync("api/bulk/Role", content);
                Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
                string userCreateDataStr = await createUserResponse.Content.ReadAsStringAsync();
                Assert.True(userCreateDataStr.Length > 0);
                OperationResultDto<RoleEntity[]> bulkCreateResult = JsonConvert.DeserializeObject<OperationResultDto<RoleEntity[]>>(userCreateDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Equal(roles.Length, bulkCreateResult.Data.Length);
                
                // 3. Getting roles again and check quantity
                resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleEntity>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                int afterBulkCreate = result.Data.Data.Count;

                Assert.Equal(beforeBulkCreate + roles.Length, afterBulkCreate);
            }
        }
        
        [Fact]
        public async Task TestBulkUpdate()
        {
        }
        
        [Fact]
        public async Task TestBulkDelete()
        {
            using (HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<RoleEntity>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleEntity>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                int itemsBeforeBulkDelete = result.Data.Data.Count;
                int itemToRemove = (int)Math.Ceiling(result.Data.Data.Count / 2.0);
                StringBuilder sb = new StringBuilder("");
                for (int i = 0; i < itemToRemove; i++)
                {
                    if (sb.Length == 0)
                        sb.Append($"?id={result.Data.Data[i].Id}");
                    else
                    {
                        sb.Append($"&id={result.Data.Data[i].Id}");
                    }
                }

                string bulkDeleteUrl = $"api/bulk/Role/{sb}";
                HttpResponseMessage deleteUserResponse = await client.DeleteAsync(bulkDeleteUrl);
                Assert.Equal(HttpStatusCode.NoContent, deleteUserResponse.StatusCode);
                
                resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleEntity>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                int itemsAfterBulkDelete = result.Data.Data.Count;
                
                Assert.Equal(itemsBeforeBulkDelete - itemToRemove, itemsAfterBulkDelete);
            }
        }
    }
}