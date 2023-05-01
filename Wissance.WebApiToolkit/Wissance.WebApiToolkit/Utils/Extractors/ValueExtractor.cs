using System;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Wissance.WebApiToolkit.Utils.Extractors
{
    public static class ValueExtractor
    {
        public  static Tuple<T, bool> TryGetVal<T>(string value)
        {
            try
            {
                Type tType = typeof(T);
                T typedValue = (T)Convert.ChangeType(value, tType);
                return new Tuple<T, bool>(typedValue, true);
            }
            catch (Exception e)
            {
                return new Tuple<T, bool>(default(T), false);
            }
            
        }

        public static Tuple<T[], bool> TryGetArray<T>(string value)
        {
            string[] parts = value.Split(",");
            T[] values = new T[parts.Length];
            for (int i=0; i < values.Length; i++)
            {
                Tuple<T, bool> item = TryGetVal<T>(parts[i]);
                if (!item.Item2)
                    return new Tuple<T[], bool>(null, false);
                values[i] = item.Item1;
            }

            return new Tuple<T[], bool>(values, true);
        }
    }
}