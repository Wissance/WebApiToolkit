using Wissance.WebApiToolkit.Core.Controllers;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Managers;

namespace Wissance.WebApiToolkit.Ef.Controllers
{
    public class GenericBulkController<TRes, TData, TId, TFilter>: BasicBulkCrudController<TRes, TData, TId, TFilter>
        where TRes : class
        where TFilter: class, IReadFilterable
    {
        public GenericBulkController(IModelManager<TRes, TData, TId> manager)
        {
            Manager = manager;
        }
    }
}