using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services
{
    /// <summary>
    ///    This is a Resource based CRUD Service interface to interact in a Resource-oriented way
    ///    via GRPC, could be used also for others Frameworks &amp;&amp; Protocols like SignalR &amp;&amp; WCF. This interface should be
    ///    implemented in some real service classes. Examples will be here: https://github.com/Wissance/WeatherControl 
    /// </summary>
    /// <typeparam name="TRes">TRes (Resource) means Representation of Persistent data in external system i.e. DTO</typeparam>
    /// <typeparam name="TData">Persistent item type, in terms of Web App it is a Table or some ORM Entity Class</typeparam>
    /// <typeparam name="TId">Unique Identifier type (could be different for different apps i.e int/string/Guid)</typeparam>
    public interface IResourceBasedCrudService<TRes, TData, TId> : IResourceBasedReadOnlyService<TRes, TData, TId>
        where TRes: class
    {
        Task<OperationResultDto<TRes>> CreateAsync(TRes data);
        Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data);
        Task<OperationResultDto<bool>> DeleteAsync(TId id);
    }
}