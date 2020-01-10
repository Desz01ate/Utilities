using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Enum
{
    /// <summary>
    /// Standard numeric format strings are used to format common numeric types (https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-currency-c-format-specifier)
    /// </summary>
    public sealed class FormatSpecifier
    {
        /// <summary>
        /// Inner value for current format.
        /// </summary>
        public readonly string Value;
        private FormatSpecifier(string value)
        {
            this.Value = value;
        }
        private static readonly Lazy<FormatSpecifier> _lazyCurrency = new Lazy<FormatSpecifier>(() => new FormatSpecifier("C"));
        private static readonly Lazy<FormatSpecifier> _lazyDecimal = new Lazy<FormatSpecifier>(() => new FormatSpecifier("D"));
        private static readonly Lazy<FormatSpecifier> _lazyExponential = new Lazy<FormatSpecifier>(() => new FormatSpecifier("E"));
        private static readonly Lazy<FormatSpecifier> _lazyFixedPoint = new Lazy<FormatSpecifier>(() => new FormatSpecifier("F"));
        private static readonly Lazy<FormatSpecifier> _lazyGeneral = new Lazy<FormatSpecifier>(() => new FormatSpecifier("G"));
        private static readonly Lazy<FormatSpecifier> _lazyNumeric = new Lazy<FormatSpecifier>(() => new FormatSpecifier("N"));
        private static readonly Lazy<FormatSpecifier> _lazyPercent = new Lazy<FormatSpecifier>(() => new FormatSpecifier("P"));
        private static readonly Lazy<FormatSpecifier> _lazyRoundTrip = new Lazy<FormatSpecifier>(() => new FormatSpecifier("R"));
        private static readonly Lazy<FormatSpecifier> _lazyHexadecimal = new Lazy<FormatSpecifier>(() => new FormatSpecifier("X"));
        /// <summary>
        /// The "C" (or currency) format specifier converts a number to a string that represents a currency amount. 
        /// The precision specifier indicates the desired number of decimal places in the result string. 
        /// If the precision specifier is omitted, the default precision is defined by the NumberFormatInfo.CurrencyDecimalDigits property.
        /// </summary>
        public static FormatSpecifier Currency => _lazyCurrency.Value;
        /// <summary>
        /// The "D" (or decimal) format specifier converts a number to a string of decimal digits (0-9), prefixed by a minus sign if the number is negative. 
        /// This format is supported only for integral types.
        /// </summary>
        public static FormatSpecifier Decimal => _lazyDecimal.Value;
        /// <summary>
        /// The exponential ("E") format specifier converts a number to a string of the form "-d.ddd…E+ddd" or "-d.ddd…e+ddd", where each "d" indicates a digit (0-9). 
        /// The string starts with a minus sign if the number is negative. 
        /// Exactly one digit always precedes the decimal point.
        /// </summary>
        public static FormatSpecifier Exponential => _lazyExponential.Value;
        /// <summary>
        /// The fixed-point ("F") format specifier converts a number to a string of the form "-ddd.ddd…" where each "d" indicates a digit (0-9). 
        /// The string starts with a minus sign if the number is negative.
        /// </summary>
        public static FormatSpecifier FixedPoint => _lazyFixedPoint.Value;
        /// <summary>
        /// The general ("G") format specifier converts a number to the more compact of either fixed-point or scientific notation, depending on the type of the number and whether a precision specifier is present. 
        /// The precision specifier defines the maximum number of significant digits that can appear in the result string. 
        /// If the precision specifier is omitted or zero, the type of the number determines the default precision, as indicated in the following table.
        /// </summary>
        public static FormatSpecifier General => _lazyGeneral.Value;
        /// <summary>
        /// The numeric ("N") format specifier converts a number to a string of the form "-d,ddd,ddd.ddd…", where "-" indicates a negative number symbol if required, "d" indicates a digit (0-9), "," indicates a group separator, and "." indicates a decimal point symbol. 
        /// The precision specifier indicates the desired number of digits after the decimal point. 
        /// If the precision specifier is omitted, the number of decimal places is defined by the current NumberFormatInfo.NumberDecimalDigits property.
        /// </summary>
        public static FormatSpecifier Numeric => _lazyNumeric.Value;
        /// <summary>
        /// The percent ("P") format specifier multiplies a number by 100 and converts it to a string that represents a percentage. 
        /// The precision specifier indicates the desired number of decimal places. 
        /// If the precision specifier is omitted, the default numeric precision supplied by the current PercentDecimalDigits property is used.
        /// </summary>
        public static FormatSpecifier Percent => _lazyPercent.Value;
        /// <summary>
        /// The round-trip ("R") format specifier attempts to ensure that a numeric value that is converted to a string is parsed back into the same numeric value. 
        /// This format is supported only for the Single, Double, and BigInteger types.
        /// </summary>
        public static FormatSpecifier RoundTrip => _lazyRoundTrip.Value;
        /// <summary>
        /// The hexadecimal ("X") format specifier converts a number to a string of hexadecimal digits. 
        /// The case of the format specifier indicates whether to use uppercase or lowercase characters for hexadecimal digits that are greater than 9. 
        /// For example, use "X" to produce "ABCDEF", and "x" to produce "abcdef". This format is supported only for integral types.
        /// </summary>
        public static FormatSpecifier Hexadecimal => _lazyHexadecimal.Value;
    }
}
