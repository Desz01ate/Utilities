using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Shared
{
    public static class Enumerator
    {
        public static IEnumerable<T> CombineEnumerator<T>(params IEnumerable<T>[] enumerables) => enumerables.SelectMany(i => i);
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> baseEnumerable, int startIndex, int count)
        {
            T[] result = new T[count];
            Array.Copy(baseEnumerable.ToArray(), startIndex, result, 0, count);
            return result;
        }
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> baseEnumerable, int count)
        {
            return baseEnumerable.Skip(Math.Max(0, baseEnumerable.Count() - count));
        }
    }
}
