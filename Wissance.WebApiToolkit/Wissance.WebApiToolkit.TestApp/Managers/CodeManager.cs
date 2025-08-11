
using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Ef.Managers;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Managers
{
    public class CodeManager : EfModelManager<CodeDto, CodeEntity, int>
    {
        public CodeManager(ModelContext dbContext, Func<CodeEntity, IDictionary<string, string>, bool> filterFunc, Func<CodeEntity, CodeDto> createFunc, 
                           ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createFunc, null, null, loggerFactory)
        {
        } 
    }
}