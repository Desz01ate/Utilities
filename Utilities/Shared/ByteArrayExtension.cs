using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Shared
{
    public static class ByteArrayExtension
    {
        public static string ToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
