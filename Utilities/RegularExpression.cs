using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class RegularExpression
    {
        private static Match RegexMatch(string input, string pattern)
        {
            return new Regex(pattern).Match(input);
        }
        /// <summary>
        /// Check if the given input is matching the phone number (Thailand phone number is default)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="pattern">Regex pattern to check whether the input is matched or not</param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string input, string pattern = @"^(08|\+66)\d{8,9}$") => RegexMatch(input, pattern).Success;
        /// <summary>
        /// Check if the given input is matching the email
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="pattern">Regex pattern to check whether the input is matched or not</param>
        /// <returns></returns>
        public static bool IsEmail(string input, string pattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?") => RegexMatch(input, pattern).Success;
        /// <summary>
        /// Check if the given input is matching the number-only
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        public static bool IsOnlyDigit(string input) => RegexMatch(input, @"^\d+$").Success;
        //\u0E00-\u0E7F isa unicode for Thai language
        /// <summary>
        /// Check if the given input is matching the string-only (English and Thai alphabetics only)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        public static bool IsOnlyText(string input) => RegexMatch(input, @"^[\u0E00-\u0E7Fa-zA-Z]+$").Success;

    }
}
