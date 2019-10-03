using System;
using System.Collections.Generic;

namespace Utilities.Shared
{
    internal static class GenericExtension
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
    }
}