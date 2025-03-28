using Grpc.Core;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Generated;

namespace Wissance.WebApiToolkit.TestApp.WebServices.Grpc
{
    public class CodeGrpcService : CodeService.CodeServiceBase
    {
        public CodeGrpcService()
        {
        }

        public override async Task<CodePagedDataOperationResult> ReadMany(PageDataRequest request, ServerCallContext context)
        {
            return await base.ReadMany(request, context);
        }

        public override async Task<CodeOperationResult> ReadOne(OneItemRequest request, ServerCallContext context)
        {
            return await base.ReadOne(request, context);
        }
    }
}