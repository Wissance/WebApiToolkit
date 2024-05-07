using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Controllers
{
    public abstract class BasicCrudController <TRes, TData, TId, TFilter> : BasicReadController<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class
    {
        /// <summary>
        /// Creates a new object
        /// </summary>
        /// <param name="data">The new object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]")]
        public virtual async Task<OperationResultDto<TRes>> CreateAsync([FromBody] TRes data)
        {
            OperationResultDto<TRes> result = await Manager.CreateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        /// <summary>
        /// Updates an object
        /// </summary>
        /// <param name="id">Identifier of an object to update</param>
        /// <param name="data">The object data</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/[controller]/{id}")]
        public virtual async Task<OperationResultDto<TRes>> UpdateAsync([FromRoute] TId id, [FromBody] TRes data)
        {
            OperationResultDto<TRes> result = await Manager.UpdateAsync(id, data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        /// <summary>
        /// Deletes an object by identifier
        /// </summary>
        /// <param name="id">Identifier of the object to delete</param>
        /// <returns></returns>
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
