using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.Tests.Utils.Operations
{
    internal static class TestBasicHttpInteraction
    {
        public static async Task<OperationResultDto<PagedDataDto<T>>> ExecReadManyAndCheckAsync<T>(HttpClient client, string url,
                                                                                                   HttpStatusCode expectedStatusCode) where T: class
        {
            HttpResponseMessage resp = await client.GetAsync(url);
            Assert.Equal(expectedStatusCode, resp.StatusCode);
            string pagedDataStr = await resp.Content.ReadAsStringAsync();
            Assert.True(pagedDataStr.Length > 0);
            if (expectedStatusCode == HttpStatusCode.OK)
            {
                OperationResultDto<PagedDataDto<T>> result = JsonConvert.DeserializeObject<OperationResultDto<PagedDataDto<T>>>(pagedDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                return result;
            }

            return null;
        }

        public static async Task<OperationResultDto<T>> ExecCreateAndCheckAsync<T>(HttpClient client, string url, T dto,
            HttpStatusCode expectedStatusCode) where T : class
        {
            JsonContent content = JsonContent.Create(dto);
            HttpResponseMessage resp = await client.PostAsync(url, content);
            Assert.Equal(expectedStatusCode, resp.StatusCode);
            string createdDataStr = await resp.Content.ReadAsStringAsync();
            Assert.True(createdDataStr.Length > 0);
            if (expectedStatusCode == HttpStatusCode.Created)
            {
                OperationResultDto<T> result = JsonConvert.DeserializeObject<OperationResultDto<T>>(createdDataStr);
                Assert.NotNull(result);
                Assert.True(result.Success);
                return result;
            }

            return null;
        }

    }
}