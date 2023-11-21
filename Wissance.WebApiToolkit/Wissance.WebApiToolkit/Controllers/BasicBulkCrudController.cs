using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Controllers
{
    public class BasicBulkCrudController<TRes, TData, TId, TQueryArguments> : BasicReadController<TRes, TData, TId, TQueryArguments>
        where TRes : class
        where TQueryArguments: class, new()
    {
        [HttpPost]
        [Route("api/bulk/[controller]")]
        public virtual async Task<OperationResultDto<TRes[]>> BulkCreateAsync([FromBody] TRes[] data)
        {
            OperationResultDto<TRes[]> result = await Manager.BulkCreateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpPut]
        [Route("api/bulk/[controller]")]
        public virtual async Task<OperationResultDto<TRes[]>> UpdateAsync([FromBody] TRes[] data)
        {
            OperationResultDto<TRes[]> result = await Manager.BulkUpdateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpDelete]
        [Route("api/bulk/[controller]")]
        public virtual async Task DeleteAsync([FromQuery] TId[] id)
        {
            OperationResultDto<bool> result = await Manager.BulkDeleteAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return;
        }
    }
}
