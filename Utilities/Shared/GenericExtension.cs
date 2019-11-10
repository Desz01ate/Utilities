using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Utilities.Classes;

namespace Utilities.Shared
{
    public static partial class GenericExtension
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
    public static partial class GenericExtension
    {
        private static ConcurrentDictionary<Type, int> _cache = new ConcurrentDictionary<Type, int>();
#if NET45
        private static readonly Type[] emptyTypeArray = new Type[0];
#else
        private static readonly Type[] emptyTypeArray = Array.Empty<Type>();
#endif
        private static int GetTypeSize(Type type)
        {
            //skip reference type, which can't identify size due to pointer.
            if (type.IsClass)
            {
                return 0;
            }
            return _cache.GetOrAdd(type, _ =>
            {
                var dm = new DynamicMethod("SizeOfType", typeof(int), emptyTypeArray);
                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Sizeof, type);
                il.Emit(OpCodes.Ret);
                return (int)dm.Invoke(null, null);
            });
        }
        /// <summary>
        /// Get size of class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="measureSize">Measure size, default = byte</param>
        /// <param name="measureRatio">Measure ratio, default = 1024.</param>
        /// <returns></returns>
        public static float SizeOf<T>(Enum.MeasureSize measureSize = Enum.MeasureSize.Byte, float measureRatio = 1024) where T : class
        {
            var type = typeof(T);
            return SizeOf(type, measureSize, measureRatio);
        }
        public static float SizeOf(object obj, Enum.MeasureSize measureSize = Enum.MeasureSize.Byte, float measureRatio = 1024)
        {
            if (obj is null)
            {
                throw new NullReferenceException("object must not be null.");
            }
            var type = obj.GetType();
            while (type.IsNested)
                type = type.BaseType;
            return SizeOf(type, measureSize, measureRatio);

        }
        /// <summary>
        /// Get size of class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">Target type</param>
        /// <param name="measureSize">Measure size, default = byte</param>
        /// <param name="measureRatio">Measure ratio, default = 1024.</param>
        /// <returns></returns>
        public static float SizeOf(Type type, Enum.MeasureSize measureSize = Enum.MeasureSize.Byte, float measureRatio = 1024)
        {
            if (type is null)
            {
                throw new NullReferenceException("Type must not be null.");
            }
            var publicProperties = type.GetProperties();
            var privateProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
            var properties = publicProperties.Merge(privateProperties);
            var size = properties.Select(x => x.PropertyType).Select(GetTypeSize).Sum();
            return measureSize switch
            {
                Enum.MeasureSize.Byte => size,
                Enum.MeasureSize.Kilobyte => size / measureRatio,
                Enum.MeasureSize.Megabyte => size / measureRatio / measureRatio,
                _ => throw new NotImplementedException($"{measureSize.ToString()} is not implemented on GenericExtension.SizeOf method.")
            };
        }

    }
}