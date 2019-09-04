using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> dataset, params IEnumerable<T>[] enumerables) => dataset.Concat(enumerables.SelectMany(x => x));
        /// <summary>
        /// Create new enumerable from given enumerable, start index and count
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="baseEnumerable">base enumerable</param>
        /// <param name="startIndex">starting index</param>
        /// <param name="count">count of elements</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="Exception"/>
        /// <returns></returns>
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> baseEnumerable, int startIndex, int count)
        {
            var totalElement = baseEnumerable.Count();
            if (baseEnumerable == null)
            {
                throw new ArgumentNullException("Base dataset must not be null.");
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("Start index must be positive-base number.");
            }
            if (totalElement < (count - startIndex))
            {
                throw new ArgumentOutOfRangeException("Count must not exceed total element of dataset.");
            }
            T[] result = new T[count];
            Array.Copy(baseEnumerable.ToArray(), startIndex, result, 0, count);
            return result;
        }
        /// <summary>
        /// Take last element out of given enumerable
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="baseEnumerable">base enumerable</param>
        /// <param name="count">count of elements</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <returns></returns>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> baseEnumerable, int count)
        {
            var totalElement = baseEnumerable.Count();
            if (totalElement < count)
            {
                throw new ArgumentOutOfRangeException($"Count must not exceed total element of dataset.");
            }
            return baseEnumerable.Skip(Math.Max(0, totalElement - count));
        }
        /// <summary>
        /// Convert given collection to list **only if** enumerable is currently not a list, otherwise return data without mutable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> AsList<T>(this IEnumerable<T> source)
        {
            if (source == null || source is List<T>)
            {
                return (List<T>)source;
            }
            return source.ToList();
        }
        /// <summary>
        /// Splits the collection into two collections, which is paired as Match and Unmatch.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataset">A base dataset.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public static (IEnumerable<T> Match, IEnumerable<T> Unmatch) Partition<T>(this IEnumerable<T> dataset, Func<T, bool> predicate)
        {
            var match = dataset.Where(predicate);
            var unmatch = dataset.Except(match);
            return (match, unmatch);
        }
        /// <summary>
        /// Convert IEnuemrable into Stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataset"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> dataset)
        {
            if (dataset == null)
            {
                throw new ArgumentNullException("Dataset must not be null");
            }
            var stack = new Stack<T>();
            foreach (var data in dataset)
            {
                stack.Push(data);
            }
            return stack;
        }
        /// <summary>
        /// Convert IEnumerable into Queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataset"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> dataset)
        {
            if (dataset == null)
            {
                throw new ArgumentNullException("Dataset must not be null");
            }
            var queue = new Queue<T>();
            foreach (var data in dataset)
            {
                queue.Enqueue(data);
            }
            return queue;
        }
    }
}
