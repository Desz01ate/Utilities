using System;
using System.Linq;

namespace HighPerformance
{
    public static class String
    {
        public static ReadOnlySpan<char> SliceString(this string text, int startIndex)
        {
            var spanText = text.AsSpan();
            return spanText.Slice(startIndex);
        }
        public static ReadOnlySpan<char> SliceString(this string text, int startIndex, int length)
        {
            var spanText = text.AsSpan();
            return spanText.Slice(startIndex, length);
        }
    }
}
