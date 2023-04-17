using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Data.EfContext
{
    public abstract class EfDbContext : DbContext, IEfDbSetResolver
    {
        public abstract DbSet<T> Get<T, TId>() where T : class, IModelIdentifiable<TId>;
    }
}