
using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Managers;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Managers
{
    public class CodeManager : EfModelManager<CodeDto, CodeEntity, int>
    {
        public CodeManager(DbContext dbContext, Func<CodeEntity, IDictionary<string, string>, bool> filterFunc, Func<CodeEntity, CodeDto> createFunc, 
                           ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createFunc, loggerFactory)
        {
        }
    }
}