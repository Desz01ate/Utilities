using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Shared
{
    public static class LINQ
    {
        public static T[] Combine<T>(params IEnumerable<T>[] items) => items.SelectMany(i => i).ToArray();
    }
}
