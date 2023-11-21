using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Controllers
{
    public abstract class BasicCrudController<TRes, TData, TId, TQueryArguments> : BasicReadController<TRes, TData, TId, TQueryArguments>
        where TRes : class
        where TQueryArguments : class, new()
    {
        [HttpPost]
        [Route("api/[controller]")]
        public virtual async Task<OperationResultDto<TRes>> CreateAsync([FromBody] TRes data)
        {
            OperationResultDto<TRes> result = await Manager.CreateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpPut]
        [Route("api/[controller]/{id}")]
        public virtual async Task<OperationResultDto<TRes>> UpdateAsync([FromRoute] TId id, [FromBody] TRes data)
        {
            OperationResultDto<TRes> result = await Manager.UpdateAsync(id, data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public virtual async Task DeleteAsync([FromRoute] TId id)
        {
            OperationResultDto<bool> result = await Manager.DeleteAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return;
        }

    }
}
