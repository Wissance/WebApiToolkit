using System.Threading.Tasks;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Generated;
using Wissance.WebApiToolkit.Tests.Utils;

namespace Wissance.WebApiToolkit.Tests.Services
{
    public class TestResourceBasedDataManageableReadOnlyService : WebApiTestBasedOnTestApp
    {

        [Theory]
        [InlineData(1, 25, 1)]
        [InlineData(1, 2, 2)]
        public async Task TestReadAsync(int page, int size, int expectedPages)
        {
            CodeService.CodeServiceClient client = new CodeService.CodeServiceClient(CreateChannel());
            PageDataRequest request = new PageDataRequest()
            {
                Page = page,
                Size = size
            };
            
            CodePagedDataOperationResult response = await client.ReadManyAsync(request);
            Assert.True(response.Success);
        }
    }
}