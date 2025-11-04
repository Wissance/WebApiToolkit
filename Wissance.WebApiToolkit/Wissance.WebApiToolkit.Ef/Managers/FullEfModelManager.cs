using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    public class FullEfModelManager<TRes, TObj, TId> : EfModelManager<TRes, TObj, TId>
        where TRes : class
        where TObj : class, IModelIdentifiable<TId>
        where TId : IComparable
    {
        public FullEfModelManager(DbContext dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc,
            Func<TObj, TRes> createResFunc, Func<TRes, TObj> createObjFunc, Action<TRes, TId, TObj> updateObjFunc,
            ILoggerFactory loggerFactory)
            : base(dbContext, filterFunc, createResFunc, createObjFunc, updateObjFunc, loggerFactory)
        {
        }
    }
}