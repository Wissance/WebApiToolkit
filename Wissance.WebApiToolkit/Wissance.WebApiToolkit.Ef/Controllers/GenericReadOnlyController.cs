using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Managers;

namespace Wissance.WebApiToolkit.Ef.Controllers
{
    internal class GenericReadOnlyController<TRes, TData, TId, TFilter> : BasicReadController<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class, IReadFilterable
    {
        public GenericReadOnlyController(IModelManager<TRes, TData, TId> manager)
        {
            Manager = manager;
        }
    }
}