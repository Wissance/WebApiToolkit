using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.Managers;

namespace Wissance.WebApiToolkit.TestApp.Controllers
{
    public sealed class CodeController : BasicReadController<CodeDto, CodeEntity, int, EmptyAdditionalFilters>
    {
        public CodeController(CodeManager manager)
        {
            Manager = manager;
        }
    }
}