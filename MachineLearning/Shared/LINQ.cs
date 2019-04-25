using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearning.Shared
{
    public static class LINQ
    {
        public static IEnumerable<T> CombineEnumerator<T>(params IEnumerable<T>[] items) => items.SelectMany(i => i);
    }
}
