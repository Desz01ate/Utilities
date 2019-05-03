using System;

namespace HighPerformance
{
    public static class String
    {
        public static ReadOnlySpan<char> SliceString(this string text, int startIndex)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new NullReferenceException("string must not be null or empty.");
            var spanText = text.AsSpan();
            return spanText.Slice(startIndex);
        }
        public static ReadOnlySpan<char> SliceString(this string text, int startIndex, int length)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new NullReferenceException("string must not be null or empty.");
            var spanText = text.AsSpan();
            return spanText.Slice(startIndex, length);
        }
    }
}
