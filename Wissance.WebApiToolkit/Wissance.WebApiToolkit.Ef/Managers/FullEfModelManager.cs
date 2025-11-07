using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Ef.Configuration;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    public class FullEfModelManager<TCtx, TRes, TObj, TId> : EfModelManager<TCtx, TRes, TObj, TId>
        where TCtx : DbContext
        where TRes : class
        where TObj : class, IModelIdentifiable<TId>
        where TId : IComparable
    {
        public FullEfModelManager(TCtx dbContext, ManagerConfiguration<TCtx, TRes, TObj, TId> configuration,
            ILoggerFactory loggerFactory)
            : base(dbContext, configuration, loggerFactory)
        {
        }
    }
}