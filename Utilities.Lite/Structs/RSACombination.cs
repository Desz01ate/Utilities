using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Structs
{
    public struct RSACombination
    {
        public string PrivateKey { get; }
        public string PublicKey { get; }
        public RSACombination(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}
