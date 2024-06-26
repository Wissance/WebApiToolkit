﻿using System;
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
