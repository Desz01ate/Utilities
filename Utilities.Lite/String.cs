using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Collection of extension method for string datatype
    /// </summary>
    public static class String
    {
        /// <summary>
        /// Convert input string to upper case just first character (for the whole input or for each word)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="upperAllWords"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string ToLeadingUpper(this string input, bool upperAllWords = false, char seperator = ' ')
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Input string must not be null or empty.");
            if (upperAllWords)
                return string.Join(seperator.ToString(), input.Split(seperator).ToList().Select(word => $@"{word.First().ToString().ToUpper()}{word.Substring(1)}"));
            return $@"{input.First().ToString().ToUpper()}{input.Substring(1)}";
        }
        /// <summary>
        /// Convert input string to base64 format string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToBase64String(this string input)
        {
            var charArray = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(charArray);
        }
        /// <summary>
        /// Convert base64 string to original string
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string FromBase64String(this string base64String)
        {
            var charArray = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(charArray);
        }
    }
}
