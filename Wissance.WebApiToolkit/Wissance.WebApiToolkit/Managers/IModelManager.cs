using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Managers
{
    public interface IModelManager<TRes, TData, TId>
    {
        Task<OperationResultDto<TRes>> CreateAsync(TRes data);
        Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data);
        Task<OperationResultDto<bool>> DeleteAsync(TId id);
        Task<OperationResultDto<IList<TRes>>> GetAsync(int page, int size);
        Task<OperationResultDto<TRes>> GetByIdAsync(TId id);
    }
}
