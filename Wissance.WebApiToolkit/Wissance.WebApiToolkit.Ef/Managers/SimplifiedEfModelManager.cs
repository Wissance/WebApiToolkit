using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    public class SimplifiedEfModelManager<TObj, TId> : EfModelManager<TObj, TObj, TId>
        where TObj: class, IModelIdentifiable<TId>
        where TId: IComparable
    {
        public SimplifiedEfModelManager(DbContext dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc, 
            Func<TObj, TObj> createResFunc, Func<TObj, TObj> createObjFunc, Action<TObj, TId, TObj> updateObjFunc, 
            ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createResFunc, createObjFunc, updateObjFunc, loggerFactory)
        {
        }
    }
}