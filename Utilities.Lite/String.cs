using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public static string ToLeadingUpper(this ReadOnlySpan<char> input, bool upperAllWords = false, char seperator = ' ')
        {
            if (input.IsEmpty)
            {
                throw new ArgumentException("Input string must not be null or empty.");
            }
            bool upperNextChar = true;
            if (upperAllWords)
            {
                var sb = new StringBuilder();
                foreach (var chr in input)
                {
                    if (chr == seperator)
                    {
                        upperNextChar = true;
                        sb.Append(seperator);
                        continue;
                    }
                    if (upperNextChar)
                    {
                        upperNextChar = false;
                        sb.Append(char.ToUpper(chr));
                    }
                    else
                    {
                        sb.Append(chr);
                    }
                }
                return sb.ToString();
            }
            else
            {
                return $@"{char.ToUpper(input[0])}{input.Slice(1).ToString()}";
            }
        }
        /// <summary>
        /// Convert input string to upper case just first character (for the whole input or for each word)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="upperAllWords"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string ToLeadingUpper(this string input, bool upperAllWords = false, char seperator = ' ')
        {
            return ToLeadingUpper(input.AsSpan(), upperAllWords, seperator);
        }
        public static ReadOnlySpan<char> Slice(this string input, int startIndex)
        {
            return input.AsSpan().Slice(startIndex);
        }
        public static ReadOnlySpan<char> Slice(this string input, int startIndex, int count)
        {
            return input.AsSpan().Slice(startIndex, count);
        }
        /// <summary>
        /// Convert input string to base64 format string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToBase64String(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Input string must not be null or empty.");
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
            if (string.IsNullOrWhiteSpace(base64String)) throw new ArgumentException("Input string must not be null or empty.");
            var charArray = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(charArray);
        }
        /// <summary>
        /// Translate original text from given language to target language, supported language can be found in Utilities.Enum.Language.
        /// </summary>
        /// <param name="originalText">Original text.</param>
        /// <param name="fromLanguage">From language.</param>
        /// <param name="toLanguage">To language.</param>
        /// <returns></returns>
        public static string Translate(string originalText, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrWhiteSpace(originalText)) throw new ArgumentException("Input string must not be null or empty.");
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={WebUtility.UrlEncode(originalText)}";
            using var webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            var response = webClient.DownloadString(url);
            var startExtractIndex = response.IndexOf('"');
            var endExtractIndex = response.IndexOf('"', startExtractIndex + 1) - 1;
            var result = response.Substring(startExtractIndex + 1, endExtractIndex - 3);
            return result;
        }
        /// <summary>
        /// Convert given value into a number format.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="numericFormat">https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings</param>
        /// <returns></returns>
        public static string NumberFormat(decimal value, string numericFormat = "N")
        {
            return string.Format("{0:" + numericFormat + "}", value);
        }
        /// <summary>
        /// Convert given value into a number format.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="numericFormat">https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings</param>
        /// <returns></returns>
        public static string NumberFormat(int value, string numericFormat = "N") => NumberFormat((decimal)value, numericFormat);
        /// <summary>
        /// Convert given value into a number format.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="numericFormat">https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings</param>
        /// <returns></returns>
        public static string NumberFormat(double value, string numericFormat = "N") => NumberFormat((decimal)value, numericFormat);
        /// <summary>
        /// Convert given value into a number format.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="numericFormat">https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings</param>
        /// <returns></returns>
        public static string NumberFormat(float value, string numericFormat = "N") => NumberFormat((decimal)value, numericFormat);
        /// <summary>
        /// Convert given value into a number format.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="numericFormat">https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings</param>
        /// <returns></returns>
        public static string NumberFormat(string value, string numericFormat = "N")
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Input string must not be null or empty.");
            var success = decimal.TryParse(value, out var result);
            if (!success)
                throw new FormatException("String value is not a valid numeric value.");
            return NumberFormat(result, numericFormat);

        }

    }
}
