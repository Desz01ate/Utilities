using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Shared
{
    /// <summary>
    /// Collection of Array extension methods
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Sum value of 2-dimensions <seealso cref="int"/> array with locality of reference optimized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static long Sum(this int[,] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }
            long sum = 0;
            var y_dimension = arr.GetLength(1);
            var x_dimension = arr.GetLength(0);
            for (var y = 0; y < y_dimension; ++y)
            {
                for (var x = 0; x < x_dimension; ++x)
                {
                    sum += arr[y, x];
                }
            }
            return sum;
        }
        /// <summary>
        /// Sum value of 2-dimensions <seealso cref="float"/> array with locality of reference optimized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static float Sum(this float[,] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }
            float sum = 0;
            var y_dimension = arr.GetLength(1);
            var x_dimension = arr.GetLength(0);
            for (var y = 0; y < y_dimension; y++)
            {
                for (var x = 0; x < x_dimension; x++)
                {
                    sum += arr[y, x];
                }
            }
            return sum;
        }
        /// <summary>
        /// Sum value of 2-dimensions <seealso cref="double"/> array with locality of reference optimized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double Sum(this double[,] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }
            double sum = 0;
            var y_dimension = arr.GetLength(1);
            var x_dimension = arr.GetLength(0);
            for (var y = 0; y < y_dimension; y++)
            {
                for (var x = 0; x < x_dimension; x++)
                {
                    sum += arr[y, x];
                }
            }
            return sum;
        }

        /// <summary>
        /// Sum value of 3-dimensions <seealso cref="int"/> array with locality of reference optimized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static long Sum(this int[,,] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }
            long sum = 0;
            var z_dimension = arr.GetLength(2);
            var y_dimension = arr.GetLength(1);
            var x_dimension = arr.GetLength(0);
            for (var z = 0; z < z_dimension; z++)
                for (var y = 0; y < y_dimension; y++)
                    for (var x = 0; x < x_dimension; x++)
                        sum += arr[z, y, x];
            return sum;
        }
        /// <summary>
        /// Sum value of 3-dimensions <seealso cref="float"/> array with locality of reference optimized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static float Sum(this float[,,] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }
            float sum = 0;
            var z_dimension = arr.GetLength(2);
            var y_dimension = arr.GetLength(1);
            var x_dimension = arr.GetLength(0);
            for (var z = 0; z < z_dimension; z++)
                for (var y = 0; y < y_dimension; y++)
                    for (var x = 0; x < x_dimension; x++)
                        sum += arr[z, y, x];
            return sum;
        }
        /// <summary>
        /// Sum value of 3-dimensions <seealso cref="double"/> array with locality of reference optimized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double Sum(this double[,,] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }
            double sum = 0;
            var z_dimension = arr.GetLength(2);
            var y_dimension = arr.GetLength(1);
            var x_dimension = arr.GetLength(0);
            for (var z = 0; z < z_dimension; z++)
                for (var y = 0; y < y_dimension; y++)
                    for (var x = 0; x < x_dimension; x++)
                        sum += arr[z, y, x];
            return sum;
        }
    }
}
