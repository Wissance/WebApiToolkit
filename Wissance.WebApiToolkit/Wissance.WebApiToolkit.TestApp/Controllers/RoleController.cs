using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.TestApp.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Controllers
{
    public class RoleController : BasicBulkCrudController<RoleEntity, RoleEntity, int, EmptyAdditionalFilters>
    {
        public RoleController(IModelManager<RoleEntity, RoleEntity, int> manager)
        {
            Manager = manager;
        }
    }
}