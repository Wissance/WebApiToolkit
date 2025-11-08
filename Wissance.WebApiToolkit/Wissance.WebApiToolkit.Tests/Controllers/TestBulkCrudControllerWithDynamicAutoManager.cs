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
using Wissance.WebApiToolkit.TestApp.Dto;
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
                OperationResultDto<PagedDataDto<RoleDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleDto>>>(pagedDataStr);
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
                OperationResultDto<PagedDataDto<RoleDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleDto>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                int beforeBulkCreate = result.Data.Data.Count;

                // 2. Call bulk create method
                RoleEntity[] roles = new RoleEntity[]
                {
                    new RoleEntity()
                    {
                        Id = 10,
                        Name = "pm",
                    },
                    new RoleEntity()
                    {
                        Id = 11,
                        Name = "tech",
                    },
                    new RoleEntity()
                    {
                        Id = 12,
                        Name = "devops",
                    },
                    new RoleEntity()
                    {
                        Id = 13,
                        Name = "backend",
                    },
                    new RoleEntity()
                    {
                        Id = 14,
                        Name = "sys",
                    }
                };
                
                JsonContent content = JsonContent.Create(roles);
                HttpResponseMessage createRoleResponse = await client.PostAsync("api/bulk/Role", content);
                Assert.Equal(HttpStatusCode.Created, createRoleResponse.StatusCode);
                string RoleCreateDataStr = await createRoleResponse.Content.ReadAsStringAsync();
                Assert.True(RoleCreateDataStr.Length > 0);
                OperationResultDto<RoleDto[]> bulkCreateResult = JsonConvert.DeserializeObject<OperationResultDto<RoleDto[]>>(RoleCreateDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Equal(roles.Length, bulkCreateResult.Data.Length);
                
                // 3. Getting roles again and check quantity
                resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleDto>>>(pagedDataStr);
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
            using (HttpClient client = Application.CreateClient())
            {
                HttpResponseMessage resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                string pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                OperationResultDto<PagedDataDto<RoleDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleDto>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);

                RoleDto[] updatingRoles = new RoleDto[result.Data.Data.Count];
                result.Data.Data.CopyTo(updatingRoles, 0);
                string newRoleName = "advanced manager";
                foreach (RoleDto role in updatingRoles)
                {
                    role.Name = newRoleName;
                }
                
                JsonContent content = JsonContent.Create(updatingRoles);
                HttpResponseMessage updateRoleResponse = await client.PutAsync("api/bulk/Role", content);
                Assert.Equal(HttpStatusCode.OK, updateRoleResponse.StatusCode);
                string roleUpdateDataStr = await updateRoleResponse.Content.ReadAsStringAsync();
                Assert.True(roleUpdateDataStr.Length > 0);
                OperationResultDto<RoleDto[]> bulkUpdateResult = JsonConvert.DeserializeObject<OperationResultDto<RoleDto[]>>(roleUpdateDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Equal(updatingRoles.Length, bulkUpdateResult.Data.Length);
                foreach (RoleDto role in bulkUpdateResult.Data)
                {
                    Assert.Equal(newRoleName, role.Name);
                }
            }
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
                OperationResultDto<PagedDataDto<RoleDto>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleDto>>>(pagedDataStr);
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
                HttpResponseMessage deleteRoleResponse = await client.DeleteAsync(bulkDeleteUrl);
                Assert.Equal(HttpStatusCode.NoContent, deleteRoleResponse.StatusCode);
                
                resp = await client.GetAsync("api/bulk/Role");
                Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                pagedDataStr = await resp.Content.ReadAsStringAsync();
                Assert.True(pagedDataStr.Length > 0);
                result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<RoleDto>>>(pagedDataStr);
                // TODO(UMV): check very formally only that ReadAsync returns PagedData wrapped in OperationResult
                Assert.NotNull(result);
                Assert.True(result.Success);
                int itemsAfterBulkDelete = result.Data.Data.Count;
                
                Assert.Equal(itemsBeforeBulkDelete - itemToRemove, itemsAfterBulkDelete);
            }
        }
    }
}