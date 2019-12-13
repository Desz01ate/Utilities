using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Numerics
{
    /// decimal method derived from https://github.com/raminrahimzada/CSharp-Helper-Classes/blob/master/Math/DecimalMath
    public static partial class MathExtension
    {
        /// <summary>
        /// Represents the ratio of the circumference of a circle to its diameter, specified
        /// by the constant, π.
        /// </summary>
        public const decimal PI = 3.14159265358979323846264338327950288419716939937510M;

        /// <summary>
        /// Represent error tolerance rate.
        /// </summary>
        public const decimal EPSILON = 0.0000000000000000001M;

        /// <summary>
        /// represents 2*PI.
        /// </summary>
        private const decimal PIx2 = 6.28318530717958647692528676655900576839433879875021M;

        /// <summary>
        /// Represents the natural logarithmic base, specified by the constant, e.
        /// </summary>
        public const decimal E = 2.7182818284590452353602874713526624977572470936999595749M;

        /// <summary>
        /// represents PI/2
        /// </summary>
        private const decimal PIdiv2 = 1.570796326794896619231321691639751442098584699687552910487M;

        /// <summary>
        /// represents PI/4
        /// </summary>
        private const decimal PIdiv4 = 0.785398163397448309615660845819875721049292349843776455243M;

        /// <summary>
        /// represents 1.0/E
        /// </summary>
        private const decimal Einv = 0.3678794411714423215955237701614608674458111310317678M;
        /// <summary>
        /// log(10,E) factor
        /// </summary>
        private const decimal Log10Inv = 0.434294481903251827651128918916605082294397005803666566114M;

        /// <summary>
        /// Zero
        /// </summary>
        public const decimal Zero = 0.0M;

        /// <summary>
        /// One
        /// </summary>
        public const decimal One = 1.0M;

        /// <summary>
        /// Represents 0.5M
        /// </summary>
        private const decimal Half = 0.5M;

        /// <summary>
        /// Max iterations count in Taylor series
        /// </summary>
        private const int MaxIteration = 100;
        /// <summary>
        /// Returns e raised to the specified power.
        /// </summary>
        /// <param name="x">A number specifying a power.</param>
        /// <returns></returns>
        public static decimal Exp(decimal x)
        {
            var count = 0;
            while (x > One)
            {
                x--;
                count++;
            }
            while (x < Zero)
            {
                x++;
                count--;
            }
            var iteration = 1;
            var result = One;
            var fatorial = One;
            decimal cachedResult;
            do
            {
                cachedResult = result;
                fatorial *= x / iteration++;
                result += fatorial;
            } while (cachedResult != result);
            if (count != 0) result = result * PowerN(E, count);
            return result;
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// </summary>
        /// <param name="value">A double-precision floating-point number to be raised to a power.</param>
        /// <param name="pow">A double-precision floating-point number that specifies a power.</param>
        /// <returns></returns>
        public static decimal Pow(decimal value, decimal pow)
        {
            if (pow == Zero) return One;
            if (pow == One) return value;
            if (value == One) return One;

            if (value == Zero && pow == Zero) return One;

            if (value == Zero)
            {
                if (pow > Zero)
                {
                    return Zero;
                }

                throw new Exception("Invalid Operation: zero base and negative power");
            }

            if (pow == -One) return One / value;

            var isPowerInteger = IsInteger(pow);
            if (value < Zero && !isPowerInteger)
            {
                throw new Exception("Invalid Operation: negative base and non-integer power");
            }

            if (isPowerInteger && value > Zero)
            {
                int powerInt = (int)(pow);
                return PowerN(value, powerInt);
            }

            if (isPowerInteger && value < Zero)
            {
                int powerInt = (int)pow;
                if (powerInt % 2 == 0)
                {
                    return Exp(pow * Log(-value));
                }
                else
                {
                    return -Exp(pow * Log(-value));
                }
            }

            return Exp(pow * Log(value));
        }

        private static bool IsInteger(decimal value)
        {
            long longValue = (long)value;
            if (Abs(value - longValue) <= EPSILON)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Power to the integer value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static decimal PowerN(decimal value, int power)
        {
            if (power == Zero) return One;
            if (power < Zero) return PowerN(One / value, -power);

            var q = power;
            var prod = One;
            var current = value;
            while (q > 0)
            {
                if (q % 2 == 1)
                {
                    // detects the 1s in the binary expression of power
                    prod = current * prod; // picks up the relevant power
                    q--;
                }
                current *= current; // value^i -> value^(2*i)
                q /= 2;
            }

            return prod;
        }

        /// <summary>
        /// Returns the base 10 logarithm of a specified number.
        /// </summary>
        /// <param name="x">A number whose logarithm is to be found.</param>
        /// <returns>
        /// One of the values in the following table. d parameter Return value Positive The
        /// base 10 log of d; that is, log 10d. Zero System.Double.NegativeInfinity Negative
        /// System.Double.NaN Equal to System.Double.NaNSystem.Double.NaN Equal to System.Double.PositiveInfinitySystem.Double.PositiveInfinity
        /// </returns>
        public static decimal Log10(decimal x)
        {
            return Log(x) * Log10Inv;
        }

        /// <summary>
        /// Analogy of Math.Log
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Log(decimal x)
        {
            if (x <= Zero)
            {
                throw new ArgumentException("x must be greater than zero");
            }
            var count = 0;
            while (x >= One)
            {
                x *= Einv;
                count++;
            }
            while (x <= Einv)
            {
                x *= E;
                count--;
            }
            x--;
            if (x == 0) return count;
            var result = Zero;
            var iteration = 0;
            var y = One;
            var cacheResult = result - One;
            while (cacheResult != result && iteration < MaxIteration)
            {
                iteration++;
                cacheResult = result;
                y *= -x;
                result += y / iteration;
            }
            return count - result;
        }

        /// <summary>
        /// Returns the cosine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        public static decimal Cos(decimal x)
        {
            while (x > PIx2)
            {
                x -= PIx2;
            }
            while (x < -PIx2)
            {
                x += PIx2;
            }
            // now x in (-2pi,2pi)
            if (x >= PI && x <= PIx2)
            {
                return -Cos(x - PI);
            }
            if (x >= -PIx2 && x <= -PI)
            {
                return -Cos(x + PI);
            }
            x = x * x;
            //y=1-x/2!+x^2/4!-x^3/6!...
            var xx = -x * Half;
            var y = One + xx;
            var cachedY = y - One;//init cache  with different value
            for (var i = 1; cachedY != y && i < MaxIteration; i++)
            {
                cachedY = y;
                decimal factor = i * (i + i + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
                factor = -Half / factor;
                xx *= x * factor;
                y += xx;
            }
            return y;
        }

        /// <summary>
        /// Returns the tangent of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <returns></returns>
        public static decimal Tan(decimal x)
        {
            var cos = Cos(x);
            if (cos == Zero) throw new ArgumentException(nameof(x));
            return Sin(x) / cos;
        }

        /// <summary>
        /// Returns the sine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <returns></returns>
        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            var moduleOfSin = Sqrt(One - (cos * cos));
            var sineIsPositive = IsSignOfSinePositive(x);
            if (sineIsPositive) return moduleOfSin;
            return -moduleOfSin;
        }

        private static bool IsSignOfSinePositive(decimal x)
        {
            //truncating to  [-2*PI;2*PI]
            while (x >= PIx2)
            {
                x -= PIx2;
            }

            while (x <= -PIx2)
            {
                x += PIx2;
            }

            //now x in [-2*PI;2*PI]
            if (x >= -PIx2 && x <= -PI) return true;
            if (x >= -PI && x <= Zero) return false;
            if (x >= Zero && x <= PI) return true;
            if (x >= PI && x <= PIx2) return false;

            //will not be reached
            throw new ArgumentException(nameof(x));
        }

        /// <summary>
        /// Analogy of Math.Sqrt
        /// </summary>
        /// <param name="x"></param>
        /// <param name="epsilon">lasts iteration while error less than this epsilon</param>
        /// <returns></returns>
        public static decimal Sqrt(decimal x, decimal epsilon = Zero)
        {
            if (x < Zero) throw new OverflowException("Cannot calculate square root from a negative number");
            //initial approximation
            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == Zero) return Zero;
                current = (previous + x / previous) * Half;
            } while (Abs(previous - current) > epsilon);
            return current;
        }
        /// <summary>
        /// Returns the hyperbolic sine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <returns></returns>
        public static decimal Sinh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y - yy) * Half;
        }

        /// <summary>
        /// Returns the hyperbolic cosine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <returns></returns>
        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y + yy) * Half;
        }

        /// <summary>
        /// Analogy of Math.Sign
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int Sign(decimal x)
        {
            return x < Zero ? -1 : (x > Zero ? 1 : 0);
        }

        /// <summary>
        /// Returns the hyperbolic tangent of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <returns></returns>
        public static decimal Tanh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y - yy) / (y + yy);
        }

        /// <summary>
        /// Analogy of Math.Abs
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Abs(decimal x) => Math.Abs(x);

        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        /// <param name="x">A number representing a sine, where x must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <returns></returns>
        public static decimal Asin(decimal x)
        {
            if (x > One || x < -One)
            {
                throw new ArgumentException("x must be in [-1,1]");
            }
            //known values
            if (x == Zero) return Zero;
            if (x == One) return PIdiv2;
            //asin function is odd function
            if (x < Zero) return -Asin(-x);

            //my optimize trick here

            // used a math formula to speed up :
            // asin(x)=0.5*(pi/2-asin(1-2*x*x)) 
            // if x>=0 is true

            var newX = One - 2 * x * x;

            //for calculating new value near to zero than current
            //because we gain more speed with values near to zero
            if (Abs(x) > Abs(newX))
            {
                var t = Asin(newX);
                return Half * (PIdiv2 - t);
            }
            var y = Zero;
            var result = x;
            decimal cachedResult;
            var i = 1;
            y += result;
            var xx = x * x;
            do
            {
                cachedResult = result;
                result *= xx * (One - Half / (i));
                y += result / (2 * i + 1);
                i++;
            } while (cachedResult != result);
            return y;
        }

        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        /// <param name="x">A number representing a tangent.</param>
        /// <returns></returns>
        public static decimal Atan(decimal x)
        {
            if (x == Zero) return Zero;
            if (x == One) return PIdiv4;
            return Asin(x / Sqrt(One + x * x));
        }
        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        /// <param name="x">A number representing a cosine, where x must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <returns></returns>
        public static decimal Acos(decimal x)
        {
            if (x == Zero) return PIdiv2;
            if (x == One) return Zero;
            if (x < Zero) return PI - Acos(-x);
            return PIdiv2 - Asin(x);
        }

        /// <summary>
        /// Returns the angle whose tangent is the quotient of two specified numbers.
        /// for more see this
        /// <seealso cref="http://i.imgur.com/TRLjs8R.png"/>
        /// </summary>
        /// <param name="y">The y coordinate of a point.</param>
        /// <param name="x">The x coordinate of a point.</param>
        public static decimal Atan2(decimal y, decimal x)
        {
            if (x > Zero)
            {
                return Atan(y / x);
            }
            if (x < Zero && y >= Zero)
            {
                return Atan(y / x) + PI;
            }
            if (x < Zero && y < Zero)
            {
                return Atan(y / x) - PI;
            }
            if (x == Zero && y > Zero)
            {
                return PIdiv2;
            }
            if (x == Zero && y < Zero)
            {
                return -PIdiv2;
            }
            throw new ArgumentException("invalid atan2 arguments");
        }
    }
    /// <summary>
    /// Provides extension methods for specific circumstances on numerical on top of Math API
    /// </summary>
    public static partial class MathExtension
    {
        /// <summary>
        /// Returns the digit length of a specified number.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>Length of a number.</returns>
        public static double Length(this int value) => Math.Floor(Math.Log10(value < 0 ? Math.Abs(value) : value) + 1);
        /// <summary>
        /// Returns the digit length of a specified number.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>Length of a number.</returns>
        public static double Length(this double value) => Math.Floor(Math.Log10(value < 0 ? Math.Abs(value) : value) + 1);
        /// <summary>
        /// Returns the digit length of a specified number.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>Length of a number.</returns>
        public static double Length(this float value) => Math.Floor(Math.Log10(value < 0 ? Math.Abs(value) : value) + 1);
        /// <summary>
        /// Returns the digit length of a specified number.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>Length of a number.</returns>
        public static decimal Length(this decimal value) => Math.Floor(Log10(value < 0 ? Math.Abs(value) : value) + 1);
        /// <summary>
        /// Returns the digit length of a specified number.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>Length of a number.</returns>
        public static double Length(this long value) => Math.Floor(Math.Log10(value < 0 ? Math.Abs(value) : value) + 1);
        /// <summary>
        /// Returns the boolean whether the value is prime number or not.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>A boolean whether the value is prime number or not.</returns>
        public static bool IsPrimeNumber(int value)
        {
            if (value <= 1 || value == 2 || value % 2 == 0) return false;
            var primeTest = 3;
            while (Math.Pow(primeTest, 2) <= value)
            {
                if (value % primeTest == 0) return false;
                primeTest += 2;
            }
            return true;
        }
    }
}
