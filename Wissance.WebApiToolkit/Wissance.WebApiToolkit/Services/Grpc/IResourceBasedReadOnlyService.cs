using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers;

namespace Wissance.WebApiToolkit.Services.Grpc
{
    public interface IResourceBasedReadOnlyService<TRes, TData, TId>
        where TRes: class
    {
        Task<PagedDataDto<TRes>> ReadAsync(int? page, int? size, string sort, string order, IDictionary<string, string> filterParams);
        Task<TRes> ReadByIdAsync(TId id);
        
        IModelManager<TRes, TData, TId> Manager { get; set; }
    }
}