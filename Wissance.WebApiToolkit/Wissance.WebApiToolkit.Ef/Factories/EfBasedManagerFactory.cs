using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Ef.Configuration;
using Wissance.WebApiToolkit.Ef.Managers;

namespace Wissance.WebApiToolkit.Ef.Factories
{
    public static class EfBasedManagerFactory
    {
        public static IModelManager<TE, TE, TId> CreateSimplifiedManager<TCtx,TE, TId>(TCtx dbContext, Func<TE, IDictionary<string, string>, bool> filterFunc,
            ILoggerFactory loggerFactory)
            where TId : IComparable
            where TE : class, IModelIdentifiable<TId>
            where TCtx: DbContext
        {
            return new SimplifiedEfModelManager<TCtx, TE, TId>(dbContext, filterFunc, PassFactory.Create,
                PassFactory.Create, PassFactory.UpdateAll, loggerFactory);
        }

        public static IModelManager<TR, TE, TId> CreateFullyDefinedManager<TCtx, TR, TE, TId>(TCtx dbContext,
            ManagerConfiguration<TCtx, TR, TE, TId> configuration, ILoggerFactory loggerFactory)
            where TId : IComparable
            where TR : class
            where TE : class, IModelIdentifiable<TId>
            where TCtx: DbContext
        {
            return new FullEfModelManager<TCtx, TR, TE, TId>(dbContext, configuration, loggerFactory);
        }

    }
}