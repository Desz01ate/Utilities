using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Structs
{
    public struct MemoryEstimation
    {
        public long Before { get; }
        public long After { get; }
        public long Amount { get; }
        public MemoryEstimation(long before, long after)
        {
            Before = before;
            After = after;
            Amount = before - after;
        }
    }
}
