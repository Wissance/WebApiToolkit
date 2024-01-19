using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services
{
    public interface IResourceBasedCrudService<TRes, TData, TId> : IResourceBasedReadOnlyService<TRes, TData, TId>
        where TRes: class
    {
        Task<OperationResultDto<TRes>> CreateAsync(TRes data);
        Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data);
        Task DeleteAsync(TId id);
    }
}