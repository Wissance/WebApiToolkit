using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Services
{
    /// <summary>
    ///    This is a general RO-interface for object Reading. Mainly this interface is using for interact in a Resource-oriented way
    ///    via GRPC, could be used also for others Frameworks && Protocols like SignalR && WCF. This interface should be
    ///    implemented in some real service classes. Examples will be here: https://github.com/Wissance/WeatherControl
    /// </summary>
    /// <typeparam name="TRes">TRes (Resource) means Representation of Persistent data in external system i.e. DTO</typeparam>
    /// <typeparam name="TData">Persistent item type, in terms of Web App it is a Table or some ORM Entity Class</typeparam>
    /// <typeparam name="TId">Unique Identifier type (could be different for different apps i.e int/string/Guid)</typeparam>
    /// <typeparam name="TFilter">Filter class</typeparam>
    public interface IResourceBasedReadOnlyService<TRes, TData, TId, TFilter>
        where TRes: class
        where TData: IModelIdentifiable<TId>
        where TFilter: class, IReadFilterable
    {
        Task<OperationResultDto<PagedDataDto<TRes>>> ReadAsync(int? page, int? size, string sort, string order, TFilter filterParams);
        Task<OperationResultDto<TRes>> ReadByIdAsync(TId id);
    }
}