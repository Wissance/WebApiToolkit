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
            Type objType = typeof(T);
            T res = item;
            PropertyInfo[] properties = objType.GetProperties();
            IList<PropertyInfo> virtProperties = properties.Where(p => !p.GetAccessors()[0].IsVirtual && 
                                                                       !p.PropertyType.IsPrimitive).ToList();

            foreach (PropertyInfo virtProperty in virtProperties)
            {
                Type propType = virtProperty.GetType();
                IList<PropertyInfo> virtTypeProps = propType.GetProperties().Where(p => !p.GetAccessors()[0].IsVirtual && 
                    !p.PropertyType.IsPrimitive).ToList();
                foreach (PropertyInfo prop in virtTypeProps)
                {
                    prop.SetValue(res, null);
                }
            }
            return res;
        }

        // todo(UMV): move Generic on a class level and init properties via static constructor
        public static void UpdateAll<T, TId>(T item, TId id, T updatingItem)
        {
            Type objType = typeof(T);
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                prop.SetValue(updatingItem, prop.GetValue(item));
            }
        }
    }
}