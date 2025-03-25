using System.Threading.Tasks;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Services
{
    /// <summary>
    ///     This is a full CRUD service implementation based on usage IModelManager as a service class for accessing
    ///     persistent storage. This class has both Read operations (from ResourceBasedDataManageableReadOnlyService)
    ///     and Create, Update and Delete operations
    /// </summary>
    /// <typeparam name="TRes">TRes (Resource) means Representation of Persistent data in external system i.e. DTO</typeparam>
    /// <typeparam name="TData">Persistent item type, in terms of Web App it is a Table or some ORM Entity Class</typeparam>
    /// <typeparam name="TId">Unique Identifier type (could be different for different apps i.e int/string/Guid)</typeparam>
    /// <typeparam name="TFilter">Filter class</typeparam>
    public abstract class ResourceBasedDataManageableCrudService<TRes, TData, TId, TFilter> : ResourceBasedDataManageableReadOnlyService<TRes, TData, TId, TFilter>,
        IResourceBasedCrudService<TRes, TData, TId, TFilter>
        where TRes: class
        where TData: IModelIdentifiable<TId>
        where TFilter: class, IReadFilterable
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