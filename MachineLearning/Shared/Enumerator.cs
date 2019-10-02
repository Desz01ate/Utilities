using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineLearning.Shared
{
    public static class Enumerator
    {
        public static IEnumerable<T> CombineEnumerable<T>(params IEnumerable<T>[] items) => items.SelectMany(i => i);
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> baseArray, int startIndex, int count)
        {
            T[] result = new T[count];
            Array.Copy(baseArray.ToArray(), startIndex, result, 0, count);
            return result;
        }
    }
}
