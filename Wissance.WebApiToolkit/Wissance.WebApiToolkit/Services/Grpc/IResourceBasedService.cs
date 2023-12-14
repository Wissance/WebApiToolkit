using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services.Grpc
{
    public interface IResourceBasedService<TRes, TData, TId>
        where TRes: class
    {
        Task<PagedDataDto<TRes>> ReadAsync(int? page, int? size, string sort, string order);
        Task<TRes> ReadByIdAsync(TId id);
    }
}