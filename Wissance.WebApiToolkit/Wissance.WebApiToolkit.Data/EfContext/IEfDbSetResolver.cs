using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Data.EfContext
{

    public interface IEfDbSetResolver
    {
        DbSet<T> Get<T, TId>() where T : class, IModelIdentifiable<TId>;
    }
}