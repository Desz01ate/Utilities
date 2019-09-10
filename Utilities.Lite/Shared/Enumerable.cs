using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Utilities.Shared
{
    /// <summary>
    /// Collection of Emnumerable extension methods
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// Combine 2 or more enumerable of the same type into new enumerable
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="enumerables">enumerables to combine</param>
        /// <returns></returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> source, params IEnumerable<T>[] enumerables) => source.Concat(enumerables.SelectMany(x => x));
        /// <summary>
        /// Create new enumerable from given enumerable, start index and count
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="source">base enumerable</param>
        /// <param name="startIndex">starting index</param>
        /// <param name="count">count of elements</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="Exception"/>
        /// <returns></returns>
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> source, int startIndex, int count)
        {
            var totalElement = source.Count();
            if (source == null)
            {
                throw new ArgumentNullException("source must not be null.");
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("Start index must be positive-base number.");
            }
            if (totalElement < (count - startIndex))
            {
                throw new ArgumentOutOfRangeException("Count must not exceed total element of source.");
            }
            return source.Skip(startIndex).Take(count);
        }
        /// <summary>
        /// Take last element out of given enumerable
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="source">base enumerable</param>
        /// <param name="count">count of elements</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <returns></returns>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            var totalElement = source.Count();
            if (totalElement < count)
            {
                throw new ArgumentOutOfRangeException($"Count must not exceed total element of source.");
            }
            return source.Skip(Math.Max(0, totalElement - count));
        }
        /// <summary>
        /// Splits the collection into two collections, which is paired as Match and Unmatch.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A base dataset.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public static (IEnumerable<T> Match, IEnumerable<T> Unmatch) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var match = source.Where(predicate);
            var unmatch = source.Except(match);
            return (match, unmatch);
        }
        /// <summary>
        /// Convert IEnuemrable into Stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Dataset must not be null");
            }
            var stack = new Stack<T>();
            foreach (var data in source)
            {
                stack.Push(data);
            }
            return stack;
        }
        /// <summary>
        /// Convert IEnumerable into Queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Dataset must not be null");
            }
            var queue = new Queue<T>();
            foreach (var data in source)
            {
                queue.Enqueue(data);
            }
            return queue;
        }
        /// <summary>
        /// Convert IEnumerable to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var properties = typeof(T).GetProperties();
            var dt = new DataTable();
            foreach (var property in properties)
            {
                dt.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            foreach (var item in source)
            {
                DataRow dr = dt.NewRow();
                foreach (var property in properties)
                {
                    dr[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
