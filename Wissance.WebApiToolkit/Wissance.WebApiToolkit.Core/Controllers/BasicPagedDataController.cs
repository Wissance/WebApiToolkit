using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Core.Utils;

namespace Wissance.WebApiToolkit.Core.Controllers
{
    public abstract class BasicPagedDataController : ControllerBase
    {
        protected int GetPage(int? page)
        {
            return PagingUtils.GetPage(page);
        }

        protected int GetPageSize(int? size)
        {
            return PagingUtils.GetPageSize(size);
        }
    }
}
