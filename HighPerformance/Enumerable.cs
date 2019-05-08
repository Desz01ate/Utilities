using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighPerformance
{
    public static class Enumerable
    {
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> array, int startIndex, int length)
        {
            ReadOnlySpan<T> span = array.ToArray().AsSpan();
            return span.Slice(startIndex, length).ToArray();
        }
    }
}
