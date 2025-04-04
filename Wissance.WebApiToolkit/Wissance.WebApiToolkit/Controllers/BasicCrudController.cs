﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Controllers
{
    [Route("api/[controller]")]
    public abstract class BasicCrudController <TRes, TData, TId, TFilter> : BasicReadController<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class, IReadFilterable
    {
        [HttpPost]
        public virtual async Task<OperationResultDto<TRes>> CreateAsync([FromBody] TRes data)
        {
            OperationResultDto<TRes> result = await Manager.CreateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpPut("{id}")]
        public virtual async Task<OperationResultDto<TRes>> UpdateAsync([FromRoute] TId id, [FromBody] TRes data)
        {
            OperationResultDto<TRes> result = await Manager.UpdateAsync(id, data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync([FromRoute] TId id)
        {
            OperationResultDto<bool> result = await Manager.DeleteAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return;
        }

    }
}
