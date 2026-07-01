using Wissance.WebApiToolkit.Ef.Managers;
using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Managers
{

    public class ProfileManager: EfModelManager<ModelContext, ProfileDto, ProfileEntity, int>
    {
        public ProfileManager(ModelContext dbContext, Func<ProfileEntity, IDictionary<string, string>, bool> filterFunc, 
            Func<ProfileEntity, ProfileDto> createFunc, Func<ProfileDto, ModelContext, ProfileEntity>createObjFunc,
            Action<ProfileDto, int, ModelContext, ProfileEntity>updateObjFunc,
            ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createFunc, createObjFunc, updateObjFunc, loggerFactory)
        {
        } 
    }
}