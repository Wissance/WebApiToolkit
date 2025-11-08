using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Ef.Configuration;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    /// <summary>
    ///     This manager is not an abstract, simplified due to TRes and TObj are the same TObj type.
    /// </summary>
    /// <typeparam name="TCtx">Entity framework Database Context derives from DbContext</typeparam>
    /// <typeparam name="TObj">Model class implements IModelIdentifiable</typeparam>
    /// <typeparam name="TId">Identifier type that is using as database table PK</typeparam>
    public class SimplifiedEfModelManager<TCtx, TObj, TId> : EfModelManager<TCtx, TObj, TObj, TId>
        where TCtx: DbContext
        where TObj: class, IModelIdentifiable<TId>
        where TId: IComparable
    {
        /// <summary>
        ///    Constructor of default model manager requires that Model Context derives from EfDbContext
        /// </summary>
        /// <param name="dbContext">Ef Database context</param>
        /// <param name="createResFunc">Delegate (factory func) for creating DTO from Model</param>
        /// <param name="createObjFunc">Delegate (factory func) for creating Entity from DTO</param>
        /// <param name="updateObjFunc">Delegate (factory func) for updating Entity from DTO</param>
        /// <param name="filterFunc">Function that use dictionary with query params to filter result set</param>
        /// <param name="loggerFactory">Logger factory</param>
        public SimplifiedEfModelManager(TCtx dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc, 
            Func<TObj, TObj> createResFunc, Func<TObj, TCtx, TObj> createObjFunc, Action<TObj, TId, TCtx, TObj> updateObjFunc, 
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
        public SimplifiedEfModelManager(TCtx dbContext, ManagerConfiguration<TCtx, TObj, TObj, TId> configuration,
            ILoggerFactory loggerFactory) 
            : base(dbContext, configuration, loggerFactory)
        {
        }
    }
}