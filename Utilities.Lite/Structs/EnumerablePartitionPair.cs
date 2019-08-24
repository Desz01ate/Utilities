using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Structs
{
    public struct EnumerablePartitionPair<T>
    {
        public IEnumerable<T> Match { get; }
        public IEnumerable<T> Unmatch { get; }
        public EnumerablePartitionPair(IEnumerable<T> match, IEnumerable<T> unmatch)
        {
            Match = match;
            Unmatch = unmatch;
        }
    }
}
