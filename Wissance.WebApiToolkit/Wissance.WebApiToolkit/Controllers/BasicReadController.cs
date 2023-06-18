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

namespace Wissance.WebApiToolkit.Controllers
{

    public abstract class BasicReadController<TRes, TData, TId> : BasicPagedDataController
        where TRes: class
    {
        [HttpGet]
        [Route("api/[controller]")]
        public virtual async Task<PagedDataDto<TRes>> ReadAsync([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string sort, 
                                                                [FromQuery] string order)
        {
            int pageNumber = GetPage(page);
            int pageSize = GetPageSize(size);
            SortOption sorting = !string.IsNullOrEmpty(sort) ? new SortOption(sort, order) : null;
            string queryStrValue = HttpContext.Request.QueryString.Value;
            IDictionary<string, StringValues> queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryStrValue);
            IDictionary<string, string> parameters = queryDictionary.Where(kv => _paramsToOmit.All(p => !string.Equals(p, kv.Key.ToLower())))
                .ToDictionary(k => k.Key, v => v.Value.ToString());
            OperationResultDto<Tuple<IList<TRes>, long>> result = await Manager.GetAsync(pageNumber, pageSize, sorting, parameters);
            HttpContext.Response.StatusCode = result.Status;
            return new PagedDataDto<TRes>(pageNumber, result.Data.Item2, GetTotalPages(result.Data.Item2, pageSize), result.Data.Item1);
        }

        [HttpGet]
        [Route("api/[controller]/{id}")]
        public async Task<TRes> ReadByIdAsync([FromRoute] TId id)
        {
            OperationResultDto<TRes> result = await Manager.GetByIdAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return result.Data;
        }

        private long GetTotalPages(long totalItems, int pageSize)
        {
            if (pageSize <= 0)
            {
                // todo(UMV): this is hardly ever possible but add logging here for jokers
                return -1;
            }

            return (long)Math.Ceiling((double)totalItems / pageSize);
        }

        private const string PageQueryParam = "page";
        private const string SizeQueryParam = "size";
        private const string SortQueryParam = "sort";
        private const string OrderQueryParam = "order";

        private readonly IList<string> _paramsToOmit = new List<string>()
        {
            PageQueryParam, SizeQueryParam, SortQueryParam, OrderQueryParam
        };
        
        public IModelManager<TRes, TData, TId> Manager { get; set; }
    }
}
