using System.Threading.Tasks;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Generated;
using Wissance.WebApiToolkit.Tests.Utils;
using Wissance.WebApiToolkit.Tests.Utils.Checkers;

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
            Assert.Equal(expectedPages, response.Data.Pages);
        }

        [Theory]
        [InlineData(1)]
        public async Task TestReadByIdAsync(int id)
        {
            CodeService.CodeServiceClient client = new CodeService.CodeServiceClient(CreateChannel());
            OneItemRequest request = new OneItemRequest()
            {
                Id = id
            };
            CodeOperationResult response = await client.ReadOneAsync(request);
            Code expected = new Code()
            {
                Id = 1,
                Name = "Software development",
                Code_ = "1"
            };
            CodeChecker.Check(expected, response.Data);
        }
    }
}