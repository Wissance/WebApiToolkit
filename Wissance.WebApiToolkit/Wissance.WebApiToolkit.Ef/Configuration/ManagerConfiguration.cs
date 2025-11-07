using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Ef.Configuration
{
    /// <summary>
    ///     ManagerConfiguration contains a set of Delegates that are using for building DTO (TRes) from Data (TObj)
    ///     objects, for creating and updating Data objects and for the filtering Data object from DB
    /// </summary>
    /// <typeparam name="TCtx">Entity Framework Context</typeparam> 
    /// <typeparam name="TRes">T Resource = DTO</typeparam>
    /// <typeparam name="TObj">T Object = Data/Entity</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class ManagerConfiguration<TCtx, TRes, TObj, TId>
        where TCtx: DbContext
        where TObj: class, IModelIdentifiable<TId>
        where TRes: class
        where TId: IComparable
    {
        public Func<TObj, TRes> CreateResFunc { get; set; }
        public Func<TObj, IDictionary<string, string>, bool> FilterFunc { get; set; }
        public Func<TRes, TCtx, TObj> CreateObjFunc { get; set; }
        public Action<TRes, TId, TCtx, TObj> UpdateObjFunc { get; set; }
    }
}