using System.Threading.Tasks;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services
{
    /// <summary>
    ///    This is a Resource based CRUD Service interface to interact in a Resource-oriented way
    ///    via GRPC, could be used also for others Frameworks &amp;&amp; Protocols like SignalR &amp;&amp; WCF. This interface should be
    ///    implemented in some real service classes. Examples will be here: https://github.com/Wissance/WeatherControl . Contains methods
    ///    for Create, Update and Delete TData item respectively.
    /// </summary>
    /// <typeparam name="TRes">TRes (Resource) means Representation of Persistent data in external system i.e. DTO</typeparam>
    /// <typeparam name="TData">Persistent item type, in terms of Web App it is a Table or some ORM Entity Class</typeparam>
    /// <typeparam name="TId">Unique Identifier type (could be different for different apps i.e int/string/Guid)</typeparam>
    /// <typeparam name="TFilter">Filter class</typeparam>
    public interface IResourceBasedCrudService<TRes, TData, TId, TFilter> : IResourceBasedReadOnlyService<TRes, TData, TId, TFilter>
        where TRes: class
        where TData: IModelIdentifiable<TId>
        where TFilter: class, IReadFilterable
    {
        Task<OperationResultDto<TRes>> CreateAsync(TRes data);
        Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data);
        Task<OperationResultDto<bool>> DeleteAsync(TId id);
    }
}