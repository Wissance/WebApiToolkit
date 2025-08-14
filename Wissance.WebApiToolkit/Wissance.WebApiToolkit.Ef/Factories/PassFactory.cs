using System;
using System.Reflection;
using System.Security.Permissions;

namespace Wissance.WebApiToolkit.Ef.Factories
{
    public static class PassFactory
    {
        public static T Create<T>(T item)
        {
            return item;
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