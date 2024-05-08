using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers;
using Wissance.WebApiToolkit.Utils;

namespace Wissance.WebApiToolkit.Services
{
    /// <summary>
    ///    This is an implementation of IResourceBasedReadOnlyService based on usage IModelManager as a service class for accessing
    ///    persistent storage. This class has 2 Operations:
    ///        1. Read portion (page) of TData via Manager and Transform it to TRes inside Manager (see i.e. EfModelManager, EfSoftRemovableModelManager)
    ///        2. Read single item from Manager and return it TRes representation
    ///    This service class ia analog of a REST BasicReadController
    /// </summary>
    /// <typeparam name="TRes">TRes (Resource) means Representation of Persistent data in external system i.e. DTO</typeparam>
    /// <typeparam name="TData">Persistent item type, in terms of Web App it is a Table or some ORM Entity Class</typeparam>
    /// <typeparam name="TId">Unique Identifier type (could be different for different apps i.e int/string/Guid)</typeparam>
    /// <typeparam name="TFilter">Type of arguments with fields marked by FromQuery attribute</typeparam>
    public abstract class ResourceBasedDataManageableReadOnlyService<TRes, TData, TId, TFilter> : IResourceBasedReadOnlyService<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class
    {
        public virtual async Task<PagedDataDto<TRes>> ReadAsync(int? page, int? size, string sort, string order, TFilter filterParams)
        {
            int pageNumber = PagingUtils.GetPage(page);
            int pageSize = PagingUtils.GetPageSize(size);
            SortOption sorting = !string.IsNullOrEmpty(sort) ? new SortOption(sort, order) : null;
            OperationResultDto<Tuple<IList<TRes>, long>> result = await Manager.GetAsync(pageNumber, pageSize, sorting, filterParams);
            return new PagedDataDto<TRes>(pageNumber, result.Data.Item2, PagingUtils.GetTotalPages(result.Data.Item2, pageSize), result.Data.Item1);
        }

        public virtual async Task<OperationResultDto<TRes>> ReadByIdAsync(TId id)
        {
            OperationResultDto<TRes> result = await Manager.GetByIdAsync(id);
            return result;
        }
        
        public IModelManager<TRes, TData, TId, TFilter> Manager { get; set; }
    }
}