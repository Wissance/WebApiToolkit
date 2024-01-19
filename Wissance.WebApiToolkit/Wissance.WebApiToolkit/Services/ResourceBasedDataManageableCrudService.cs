using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services
{
    public class ResourceBasedDataManageableCrudService<TRes, TData, TId> : ResourceBasedDataManageableReadOnlyService<TRes, TData, TId>,
        IResourceBasedCrudService<TRes, TData, TId>
       where TRes: class
    {
        public Task<OperationResultDto<TRes>> CreateAsync(TRes data)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(TId id)
        {
            throw new System.NotImplementedException();
        }
    }
}