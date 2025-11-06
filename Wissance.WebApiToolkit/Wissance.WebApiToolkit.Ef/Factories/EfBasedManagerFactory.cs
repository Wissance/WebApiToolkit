using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Configuration;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Ef.Managers;

namespace Wissance.WebApiToolkit.Ef.Factories
{
    public static class EfBasedManagerFactory
    {
        public static IModelManager<TE, TE, TId> CreateSimplifiedManager<TE, TId>(DbContext dbContext, Func<TE, IDictionary<string, string>, bool> filterFunc,
            ILoggerFactory loggerFactory)
            where TId : IComparable
            where TE : class, IModelIdentifiable<TId>
        {
            return new SimplifiedEfModelManager<TE, TId>(dbContext, filterFunc, PassFactory.Create,
                PassFactory.Create, PassFactory.UpdateAll, loggerFactory);
        }

        public static IModelManager<TR, TE, TId> CreateFullyDefinedManager<TR, TE, TId>(DbContext dbContext,
            ManagerConfiguration<TR, TE, TId> configuration, ILoggerFactory loggerFactory)
            where TId : IComparable
            where TR : class
            where TE : class, IModelIdentifiable<TId>
        {
            return new FullEfModelManager<TR, TE, TId>(dbContext, configuration, loggerFactory);
        }

    }
}