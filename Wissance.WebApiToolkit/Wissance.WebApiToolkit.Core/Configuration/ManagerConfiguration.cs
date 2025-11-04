using System;
using System.Collections.Generic;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Core.Configuration
{
    /// <summary>
    ///     ManagerConfiguration contains a set of Delegates that are using for building DTO (TRes) from Data (TObj)
    ///     objects, for creating and updating Data objects and for the filtering Data object from DB
    /// </summary>
    /// <typeparam name="TRes"></typeparam>
    /// <typeparam name="TObj"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class ManagerConfiguration<TRes, TObj, TId>
        where TObj: class, IModelIdentifiable<TId>
        where TRes: class
        where TId: IComparable
    {
        public Func<TObj, TRes> CreateResFunc { get; set; }
        public Func<TObj, IDictionary<string, string>, bool> FilterFunc { get; set; }
        public Func<TRes, TObj> CreateObjFunc { get; set; }
        public Action<TRes, TId, TObj> UpdateObjFunc { get; set; }
    }
}