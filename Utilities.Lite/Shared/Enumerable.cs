﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Structs;

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
        public static IEnumerable<T> Merge<T>(params IEnumerable<T>[] enumerables) => enumerables.SelectMany(i => i);
        /// <summary>
        /// Create new enumerable from given enumerable, start index and count
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="baseEnumerable">base enumerable</param>
        /// <param name="startIndex">starting index</param>
        /// <param name="count">count of elements</param>
        /// <returns></returns>
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> baseEnumerable, int startIndex, int count)
        {
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
        /// <returns></returns>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> baseEnumerable, int count)
        {
            return baseEnumerable.Skip(Math.Max(0, baseEnumerable.Count() - count));
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
        /// Splits the collection into two collections, which is paired as Match and Unmatch in return EnumerablePartitionPair.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataset">A base dataset.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public static EnumerablePartitionPair<T> Partition<T>(this IEnumerable<T> dataset, Func<T, bool> predicate)
        {
            var match = dataset.Where(predicate);
            var unmatch = dataset.Except(match);
            return new EnumerablePartitionPair<T>(match, unmatch);
        }
    }
}
