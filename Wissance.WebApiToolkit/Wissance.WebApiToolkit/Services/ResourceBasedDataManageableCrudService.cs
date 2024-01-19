using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services
{
    public class ResourceBasedDataManageableCrudService<TRes, TData, TId> : ResourceBasedDataManageableReadOnlyService<TRes, TData, TId>,
        IResourceBasedCrudService<TRes, TData, TId>
       where TRes: class
    {
        public async Task<OperationResultDto<TRes>> CreateAsync(TRes data)
        {
            OperationResultDto<TRes> result = await Manager.CreateAsync(data);
            return result;
        }

        public async Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data)
        {
            OperationResultDto<TRes> result = await Manager.UpdateAsync(id, data);
            return result;
        }

        public async Task<OperationResultDto<bool>> DeleteAsync(TId id)
        {
            OperationResultDto<bool> result = await Manager.DeleteAsync(id);
            return result;
        }
    }
}