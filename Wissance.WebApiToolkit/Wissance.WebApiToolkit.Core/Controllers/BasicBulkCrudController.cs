using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Controllers
{
    [Route("api/bulk/[controller]")]
    public class BasicBulkCrudController<TRes, TData, TId, TFilter> : BasicReadController<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class, IReadFilterable
    {
        [HttpPost]
        public virtual async Task<OperationResultDto<TRes[]>> BulkCreateAsync([FromBody] TRes[] data)
        {
            OperationResultDto<TRes[]> result = await Manager.BulkCreateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpPut]
        public virtual async Task<OperationResultDto<TRes[]>> UpdateAsync([FromBody] TRes[] data)
        {
            OperationResultDto<TRes[]> result = await Manager.BulkUpdateAsync(data);
            HttpContext.Response.StatusCode = result.Status;
            return result;
        }

        [HttpDelete]
        public virtual async Task DeleteAsync([FromQuery] TId[] id)
        {
            OperationResultDto<bool> result = await Manager.BulkDeleteAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return;
        }
    }
}
