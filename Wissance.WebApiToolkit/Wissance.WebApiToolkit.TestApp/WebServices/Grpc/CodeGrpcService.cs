using Grpc.Core;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Services;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Generated;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Helpers;

namespace Wissance.WebApiToolkit.TestApp.WebServices.Grpc
{
    public class CodeGrpcService : CodeService.CodeServiceBase
    {
        public CodeGrpcService(ResourceBasedDataManageableReadOnlyService<CodeDto, CodeEntity, int, EmptyAdditionalFilters> serviceImpl)
        {
            _serviceImpl = serviceImpl;
        }

        public override async Task<CodePagedDataOperationResult> ReadMany(PageDataRequest request, ServerCallContext context)
        {
            OperationResultDto<PagedDataDto<CodeDto>> result = await _serviceImpl.ReadAsync(request.Page, request.Size, request.Sort, request.Order,
                new EmptyAdditionalFilters());
            context.Status = GrpcErrorCodeHelper.GetGrpcStatus(result.Status, result.Message);
            CodePagedDataOperationResult response = new CodePagedDataOperationResult()
            {
                Success = result.Success,
                Message = result.Message ?? String.Empty,
                Status = result.Status,
            };

            if (result.Data != null)
            {
                response.Data = new CodePagedDataResult()
                {
                    Page = result.Data.Page,
                    Pages = result.Data.Pages,
                    Total = result.Data.Total,
                    Data = {result.Data.Data.Select(c => Convert(c))}
                };
            }

            return response;
        }

        public override async Task<CodeOperationResult> ReadOne(OneItemRequest request, ServerCallContext context)
        {
            OperationResultDto<CodeDto> result = await _serviceImpl.ReadByIdAsync(request.Id);
            context.Status = GrpcErrorCodeHelper.GetGrpcStatus(result.Status, result.Message);
            CodeOperationResult response = new CodeOperationResult()
            {
                Success = result.Success,
                Message = result.Message ?? String.Empty,
                Status = result.Status,
                Data = Convert(result.Data)
            };
            return response;
        }

        private Code Convert(CodeDto dto)
        {
            if (dto == null)
                return null;
            return new Code()
            {
                Id = dto.Id,
                Code_ = dto.Code,
                Name = dto.Name
            };
        }

        private readonly ResourceBasedDataManageableReadOnlyService<CodeDto, CodeEntity, int, EmptyAdditionalFilters> _serviceImpl;
    }
}