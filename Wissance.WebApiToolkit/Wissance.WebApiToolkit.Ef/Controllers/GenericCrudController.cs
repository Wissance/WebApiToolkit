using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Managers;

namespace Wissance.WebApiToolkit.Ef.Controllers
{
    internal class GenericCrudController<TRes, TData, TId, TFilter> : BasicCrudController<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class, IReadFilterable
    {
        public GenericCrudController(IModelManager<TRes, TData, TId> manager)
        {
            Manager = manager;
        }
    }
}