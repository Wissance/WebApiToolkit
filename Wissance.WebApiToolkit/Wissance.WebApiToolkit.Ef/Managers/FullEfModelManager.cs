using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Ef.Configuration;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    /// <summary>
    ///     This is a fully defined implementation of a IModelManager, this is a not abstract class therefore
    ///     it could be used as a Manager in any controller with specifying only a Generic parameters. Unlike
    ///     other managers it has only one constructor with ManagerConfiguration
    /// </summary>
    /// <typeparam name="TCtx">Entity framework Database Context derives from DbContext</typeparam>
    /// <typeparam name="TRes">DTO class (representation of Model in other systems i.e. in frontend))</typeparam>
    /// <typeparam name="TObj">Model class implements IModelIdentifiable</typeparam>
    /// <typeparam name="TId">Identifier type that is using as database table PK</typeparam>
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