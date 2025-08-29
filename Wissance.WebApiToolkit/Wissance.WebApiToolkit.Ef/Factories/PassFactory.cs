using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace Wissance.WebApiToolkit.Ef.Factories
{
    public static class PassFactory
    {
        public static T Create<T>(T item)
        {
            T res = item;
            PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.DeclaredOnly | 
                                                                     BindingFlags.Public | 
                                                                     BindingFlags.Instance);
            IList<PropertyInfo> virtProperties = properties.Where(p => p.GetAccessors()[0].IsVirtual && 
                                                                       !p.PropertyType.IsPrimitive && 
                                                                       p.PropertyType.IsClass || p.PropertyType.IsInterface).ToList();

            foreach (PropertyInfo virtProperty in virtProperties)
            {
                virtProperty.SetValue(res, null);
                // object propertyValue = virtProperty.GetValue(res);
                // SetNullForVirtual(propertyValue);
            }
            return res;
        }

        // todo(UMV): move Generic on a class level and init properties via static constructor
        public static void UpdateAll<T, TId>(T item, TId id, T updatingItem)
            where T : class
        {
            Type objType = typeof(T);
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                prop.SetValue(updatingItem, prop.GetValue(item));
            }
        }

        private static void SetNullForVirtual<T>(T obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.DeclaredOnly | 
                                                                     BindingFlags.Public | 
                                                                     BindingFlags.Instance);
            IList<PropertyInfo> virtProperties = properties.Where(p => p.GetAccessors()[0].IsVirtual && 
                !p.PropertyType.IsPrimitive && 
                p.PropertyType.IsClass || p.PropertyType.IsInterface).ToList();

            foreach (PropertyInfo virtProperty in virtProperties)
            {
                virtProperty.SetValue(obj, null);
            }
        }
    }
}