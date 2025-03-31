using Wissance.WebApiToolkit.Controllers;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.Managers;

namespace Wissance.WebApiToolkit.TestApp.Controllers
{
    public class OrganizationController : BasicCrudController<OrganizationDto, OrganizationEntity, int, EmptyAdditionalFilters>
    {
        public OrganizationController(OrganizationManager manager)
        {
            Manager = manager;
        }
    }
}