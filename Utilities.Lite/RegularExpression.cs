using System;
using System.Text.RegularExpressions;
using Utilities.Enum;

namespace Utilities
{
    /// <summary>
    /// Basic regular expression wrapper
    /// </summary>
    public static class RegularExpression
    {
        private static bool RegexMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// Check if the given input is matching the phone number (Thailand phone number by default)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="pattern">Regex pattern to check whether the input is matched or not</param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string input, string pattern = @"^(0\d{1}|\+66)\d{8,9}$") => RegexMatch(input, pattern);

        /// <summary>
        /// Check if the given input is matching the email
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="pattern">Regex pattern to check whether the input is matched or not</param>
        /// <returns></returns>
        public static bool IsEmail(string input, string pattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?") => RegexMatch(input, pattern);

        /// <summary>
        /// Check if the given input is matching the number-only
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        public static bool IsOnlyDigit(string input) => RegexMatch(input, @"^\d+$");
        /// <summary>
        /// Check if the given input is matching the number-only within specified length.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool IsOnlyDigit(string input, int length) => RegexMatch(input, $@"^\d{{{length}}}$");
        /// <summary>
        /// Check if the given input is matching the number-only within the length boundary.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lengthFrom"></param>
        /// <param name="lengthTo"></param>
        /// <returns></returns>
        public static bool IsOnlyDigit(string input, int lengthFrom, int lengthTo) => RegexMatch(input, $@"^\d{{{lengthFrom},{lengthTo}}}");
        //\u0E00-\u0E7F is a Unicode for Thai language
        /// <summary>
        /// Check if the given input is matching the string-only (English and Thai alphabetics combination)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        public static bool IsOnlyText(string input) => RegexMatch(input, @"^[\u0E00-\u0E7Fa-zA-Z\s]+$");
        /// <summary>
        /// Check if the given input is matching the string-only in specific language (Support English and Thai language only).
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool IsOnlyText(string input, Language language) => language switch
        {
            Language.English => RegexMatch(input, @"^[a-zA-Z\s]+$"),
            Language.Thai => RegexMatch(input, @"^[\u0E00-\u0E7F\s]+$"),
            _ => throw new System.NotSupportedException($"{language.ToString()} is not supported.")
        };
        private const int ThaiIdLength = 13;

        /// <summary>
        /// Check if the given input is matching the Thai citizen id format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public static bool IsValidThaiCitizenId(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != ThaiIdLength || !IsOnlyDigit(input)) return false;
            var sum = 0d;
            for (var idx = 0; idx < ThaiIdLength - 1; idx++)
            {
                sum += double.Parse(input[idx].ToString()) * (ThaiIdLength - idx);
            }
            var formulaSummation = ((ThaiIdLength - 2) - sum % (ThaiIdLength - 2)) % 10;
            var lastDigit = double.Parse(input[ThaiIdLength - 1].ToString());
            return Math.Abs(formulaSummation - lastDigit) < 1e-10;
        }
    }
}