using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.TestApp.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Controllers
{
    public class UserController : BasicCrudController<UserEntity, UserEntity, int, EmptyAdditionalFilters>
    {
        public UserController(IModelManager<UserEntity, UserEntity, int> manager)
        {
            Manager = manager;
        }
    }
}