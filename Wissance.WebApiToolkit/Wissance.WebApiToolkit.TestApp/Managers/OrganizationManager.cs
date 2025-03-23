using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Managers;
using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Managers
{
    public class OrganizationManager : EfModelManager<OrganizationDto, OrganizationEntity, int>
    {
        public OrganizationManager(ModelContext dbContext, Func<OrganizationEntity, IDictionary<string, string>, bool> filterFunc, Func<OrganizationEntity, OrganizationDto> createFunc, ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createFunc, loggerFactory)
        {
        }
    }
}