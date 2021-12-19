using Microsoft.AspNetCore.Mvc;

namespace Wissance.WebApiToolkit.Controllers
{
    public abstract class BasicPagedDataController : ControllerBase
    {
        protected int GetPage(int? page)
        {
            int selectedPage = page ?? DefaultPage;
            return selectedPage < 1 ? 1 : selectedPage;
        }

        protected int GetPageSize(int? size)
        {
            return size ?? DefaultSize;
        }

        private const int DefaultPage = 1;
        private const int DefaultSize = 25;
    }
}
