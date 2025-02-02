using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers;
using Wissance.WebApiToolkit.Utils;

namespace Wissance.WebApiToolkit.Controllers
{

    /// <summary>
    ///    This is a basic Read Controller implementing two operations:
    ///        1. Get multiple items via api/controller/[page={20} & size={50} & sort=name & order=asc|desc ] with paging
    ///           and sorting by one column and filtering by any number of parameters, sort is a name of column/property
    ///           order is a sort order only asc and desc values are allowed.
    ///           Examples with Station controller:
    ///           - ~/api/Station to get items with default paging (page = 1, size = 25)
    ///           - ~/api/Station?page=2&size=50 to get items with paging options (page = 2, size = 50)
    ///           - ~/api/Station?page=1&size=40&sort=name&order=desc to get items with paging and sorting by column/property
    ///             name and in desc order (from Z to A)
    ///           - ~/api/Station?page=1&size=40&sort=name&order=desc&from=2022-01-01&to=2024-12-31 to get items with paging,
    ///             sorting and filter by date in range (2022-01-01, 2024-12-31)
    ///        2. Get single item via api by id, example with Station controller:
    ///           - ~/api/Station/145 to get station with id = 145
    ///     Restrictions:
    ///        1. We are working with a single type of result representation inn Response (TRes).
    ///        2. To get custom filters there should be a class implementing IReadFilter, you could just return an
    ///           empty dictionary and act with TFilter on you own way.
    ///        3. There are no option (currently) to receive all data without paging
    /// </summary>
    /// <typeparam name="TRes">Is A DTO object type to return back</typeparam>
    /// <typeparam name="TData">Is a type that represents persistent object (i.e. Entity class)</typeparam>
    /// <typeparam name="TId">Is a type of persistant object identifier</typeparam>
    /// <typeparam name="TFilter">Is a type of filter class</typeparam>
    public abstract class BasicReadController<TRes, TData, TId, TFilter> : BasicPagedDataController
        where TRes: class
        where TFilter: class, IReadFilterable
    {
        [HttpGet]
        [Route("api/[controller]")]
        public virtual async Task<PagedDataDto<TRes>> ReadAsync([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string sort, 
                                                                [FromQuery] string order, TFilter additionalFilters = null)
        {
            int pageNumber = GetPage(page);
            int pageSize = GetPageSize(size);
            SortOption sorting = !string.IsNullOrEmpty(sort) ? new SortOption(sort, order) : null;
            IDictionary<string, string> additionalQueryParams = additionalFilters != null
                                                              ? additionalFilters.SelectFilters()
                                                              : new Dictionary<string, string>();
            OperationResultDto<Tuple<IList<TRes>, long>> result = await Manager.GetAsync(pageNumber, pageSize, sorting, additionalQueryParams);
            HttpContext.Response.StatusCode = result.Status;
            return new PagedDataDto<TRes>(pageNumber, result.Data.Item2, PagingUtils.GetTotalPages(result.Data.Item2, pageSize), result.Data.Item1);
        }

        [HttpGet]
        [Route("api/[controller]/{id}")]
        public async Task<TRes> ReadByIdAsync([FromRoute] TId id)
        {
            OperationResultDto<TRes> result = await Manager.GetByIdAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return result.Data;
        }
        public IModelManager<TRes, TData, TId> Manager { get; set; }
    }
}
