using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Structs
{
    public struct HashCombination
    {
        public string Hash { get; }
        public string Salt { get; }
        public HashCombination(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }
}
