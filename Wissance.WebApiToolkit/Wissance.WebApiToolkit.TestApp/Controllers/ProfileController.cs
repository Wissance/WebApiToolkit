using Wissance.WebApiToolkit.Core.Attributes;
using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Operations;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.Managers;

namespace Wissance.WebApiToolkit.TestApp.Controllers
{
    [AllowedOperation(ControllerOperation.Read | ControllerOperation.ReadOne | 
                      ControllerOperation.Create  | ControllerOperation.Update)]
    public sealed class ProfileController : BasicCrudController<ProfileDto, ProfileEntity, int, EmptyAdditionalFilters>
    {
        public ProfileController(ProfileManager manager)
        {
            Manager = manager;
        }
    }
}