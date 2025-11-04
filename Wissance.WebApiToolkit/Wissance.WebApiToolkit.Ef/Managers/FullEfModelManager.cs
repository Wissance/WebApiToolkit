using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Configuration;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    public class FullEfModelManager<TRes, TObj, TId> : EfModelManager<TRes, TObj, TId>
        where TRes : class
        where TObj : class, IModelIdentifiable<TId>
        where TId : IComparable
    {
        public FullEfModelManager(DbContext dbContext, ManagerConfiguration<TRes, TObj, TId> configuration,
            ILoggerFactory loggerFactory)
            : base(dbContext, configuration, loggerFactory)
        {
        }
    }
}