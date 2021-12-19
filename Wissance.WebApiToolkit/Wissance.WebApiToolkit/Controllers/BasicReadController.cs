using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers;

namespace Wissance.WebApiToolkit.Controllers
{
    public class BasicReadController<TRes, TData, TId> : BasicPagedDataController
        where TRes: class
    {
        public BasicReadController()
        {
        }

        [HttpGet]
        [Route("api/[controller]")]
        public virtual async Task<PagedDataDto<TRes>> ReadAsync([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = GetPage(page);
            OperationResultDto<IList<TRes>> result = await Manager.GetAsync(pageNumber, GetPageSize(size));
            HttpContext.Response.StatusCode = result.Status;
            return new PagedDataDto<TRes>(pageNumber, result.Data);
        }

        [HttpGet]
        [Route("api/[controller]/{id}")]
        public async Task<TRes> ReadByIdAsync([FromRoute] int id)
        {
            OperationResultDto<TRes> result = await Manager.GetByIdAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return result.Data;
        }

        public IModelManager<TRes, TData, TId> Manager { get; set; }
    }
}
