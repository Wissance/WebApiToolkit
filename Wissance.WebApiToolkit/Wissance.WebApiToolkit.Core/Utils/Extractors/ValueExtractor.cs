using System;

namespace Wissance.WebApiToolkit.Core.Utils.Extractors
{
    /// <summary>
    ///    ValueExtractor is a static class that helps to extract variables of a specific type when we deal with query string.
    /// </summary>
    public static class ValueExtractor
    {
        /// <summary>
        ///    Get scalar type value of type T as a Tuple of T and bool (represents success of data extract) from string
        /// </summary>
        /// <param name="value">string representation of variable of type T</param>
        /// <typeparam name="T">type (scalar) of variable in a string</typeparam>
        /// <returns>Tuple with variable of type T (default(T) if conversion failed) and a bool as a success of </returns>
        public static Tuple<T, bool> TryGetVal<T>(string value)
        {
            try
            {
                Type tType = typeof(T);
                T typedValue = (T)Convert.ChangeType(value, tType);
                return new Tuple<T, bool>(typedValue, true);
            }
            catch (Exception)
            {
                return new Tuple<T, bool>(default(T), false);
            }
            
        }

        /// <summary>
        ///    Get vector type value of type T[] as a Tuple of T[] and bool (represents success of data extract) from string. Internally calls
        ///    scalar version of this func 
        /// </summary>
        /// <param name="value">string representation of array of variables of type T</param>
        /// <typeparam name="T">type of variable in a string</typeparam>
        /// <returns>Tuple with array of T (T[]) and a bool (parsing result)</returns>
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