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
        ///     Simplified model manager operates and returns as a Result (DTO) same objects. Thus, we don't need
        ///     a factory between TRes and TObj, but there are some issues with proxy objects (virtual). There is
        ///     a PassFactory class to make pseudo manipulation with object constructing
        /// </summary>
        /// <param name="dbContext">Database Context (Entity Framework)</param>
        /// <param name="configuration">A set of different Delegates combined into one Configuration class</param>
        /// <param name="loggerFactory">FLogger factory</param>
        public SimplifiedEfModelManager(DbContext dbContext, ManagerConfiguration<TObj, TObj, TId> configuration,
            ILoggerFactory loggerFactory) 
            : base(dbContext, configuration, loggerFactory)
        {
        }
    }
}