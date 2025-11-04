using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Configuration;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    /// <summary>
    ///     TODO(umv): Write
    /// </summary>
    /// <typeparam name="TObj"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class SimplifiedEfModelManager<TObj, TId> : EfModelManager<TObj, TObj, TId>
        where TObj: class, IModelIdentifiable<TId>
        where TId: IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="filterFunc"></param>
        /// <param name="createResFunc"></param>
        /// <param name="createObjFunc"></param>
        /// <param name="updateObjFunc"></param>
        /// <param name="loggerFactory"></param>
        public SimplifiedEfModelManager(DbContext dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc, 
            Func<TObj, TObj> createResFunc, Func<TObj, TObj> createObjFunc, Action<TObj, TId, TObj> updateObjFunc, 
            ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createResFunc, createObjFunc, updateObjFunc, loggerFactory)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public SimplifiedEfModelManager(DbContext dbContext, ManagerConfiguration<TObj, TObj, TId> configuration,
            ILoggerFactory loggerFactory) 
            : base(dbContext, configuration, loggerFactory)
        {
        }
    }
}