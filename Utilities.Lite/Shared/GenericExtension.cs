using System;
using System.Collections.Generic;
using Utilities.Classes;

namespace Utilities.Shared
{
    public static class GenericExtension
    {
        /// <summary>
        /// Verify if the object is a generic list.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        internal static bool IsGenericList(this object o)
        {
            var type = o.GetType();
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }

        /// <summary>
        /// Internally convert object to target type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static dynamic Cast(dynamic obj, Type targetType)
        {
            return Convert.ChangeType(obj, targetType);
        }
        /// <summary>
        /// Analyze structure of dynamic object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DynamicObjectMetadataCollection DynamicObjectAnalyzer(dynamic obj)
        {
            if (obj is null)
            {
                return null;
            }
            var result = new DynamicObjectMetadataCollection();
            if (obj is IDictionary<string, object> dict)
            {
                foreach (var pair in dict)
                {
                    result.Add(pair.Value?.GetType(), pair.Key, pair.Value, pair);
                }
            }
            return result;
        }
    }
}