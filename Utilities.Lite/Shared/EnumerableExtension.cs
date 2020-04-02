using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Shared
{
    /// <summary>
    /// Collection of Enumerable extension methods
    /// </summary>
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Combine 2 or more enumerable of the same type into new enumerable
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="enumerables">enumerables to combine</param>
        /// <returns></returns>
        [Obsolete("This method is deprecated, please use Concat instead.")]
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> source, params IEnumerable<T>[] enumerables) => source.Concat(enumerables.SelectMany(x => x));

        /// <summary>
        /// Concatenates two to N sequences.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The first sequence to concatenate.</param>
        /// <param name="elements">The sequence to concatenate to the first sequence.</param>
        /// <returns></returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params IEnumerable<T>[] elements)
        {
            if (source.IsNullOrEmpty() && elements.IsNullOrEmpty())
            {
                return Enumerable.Empty<T>();
            }
            return Enumerable.Concat(source, elements.SelectMany(x => x));
        }
        /// <summary>
        /// Create new enumerable from given enumerable, start index and count
        /// </summary>
        /// <typeparam name="T">type of enumerable</typeparam>
        /// <param name="source">base enumerable</param>
        /// <param name="startIndex">starting index</param>
        /// <param name="count">count of elements</param>

        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="Exception"/>
        /// <returns></returns>
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> source, int startIndex, int count)
        {
            var totalElement = source.Count();
            if (source.IsNullOrEmpty())
            {
                return Enumerable.Empty<T>();
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (totalElement < (count - startIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return source.Skip(startIndex).Take(count);
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
            if (source.IsNullOrEmpty()) return (Enumerable.Empty<T>(), Enumerable.Empty<T>());
            var match = source.Where(predicate);
            var unmatch = source.Except(match);
            return (match, unmatch);
        }
        /// <summary>
        /// Make an iterator that aggregates elements from each of the iterables.
        /// Returns an iterator of tuples, where the i-th tuple contains the i-th element from each of the argument
        /// sequences or iterables.The iterator stops when the shortest input iterable is exhausted.With a single
        /// iterable argument, it returns an iterator of 1-tuples.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            if (source1.IsNullOrEmpty() || source2.IsNullOrEmpty())
            {
                return Enumerable.Empty<(T1, T2)>();
            }
            return source1.Zip(source2, (x, y) => (t1: x, t2: y));
        }
        /// <summary>
        /// Make an iterator that aggregates elements from each of the iterables.
        /// Returns an iterator of tuples, where the i-th tuple contains the i-th element from each of the argument
        /// sequences or iterables.The iterator stops when the shortest input iterable is exhausted.With a single
        /// iterable argument, it returns an iterator of 1-tuples.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <returns></returns>
        public static IEnumerable<(T1, T2, T3)> Zip<T1, T2, T3>(IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3)
        {
            if (source1.IsNullOrEmpty() || source2.IsNullOrEmpty() || source3.IsNullOrEmpty())
            {
                return Enumerable.Empty<(T1, T2, T3)>();
            }
            var z1 = source1.Zip(source2, (x, y) => (x, y));
            var z2 = z1.Zip(source3, (x, y) => (t1: x.x, t2: x.y, t3: y));
            return z2;
        }
        /// <summary>
        /// Make an iterator that aggregates elements from each of the iterables.
        /// Returns an iterator of tuples, where the i-th tuple contains the i-th element from each of the argument
        /// sequences or iterables.The iterator stops when the shortest input iterable is exhausted.With a single
        /// iterable argument, it returns an iterator of 1-tuples.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <param name="source4"></param>
        /// <returns></returns>
        public static IEnumerable<(T1, T2, T3, T4)> Zip<T1, T2, T3, T4>(IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4)
        {
            if (source1.IsNullOrEmpty() || source2.IsNullOrEmpty() || source3.IsNullOrEmpty() || source4.IsNullOrEmpty())
            {
                return Enumerable.Empty<(T1, T2, T3, T4)>();
            }
            var z1 = source1.Zip(source2, (x, y) => (x, y));
            var z2 = z1.Zip(source3, (x, y) => (x.x, x.y, y));
            var z3 = z2.Zip(source4, (x, y) => (t1: x.x, t2: x.Item2, t3: x.Item3, t4: y));
            return z3;
        }
        /// <summary>
        /// Convert IEnuemrable into Stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>

        /// <returns></returns>
        public static Stack<T>? ToStack<T>(this IEnumerable<T> source)
        {
            if (source == null) return null;
            var stack = new Stack<T>(source);
            return stack;
        }

        /// <summary>
        /// Convert IEnumerable into Queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>

        /// <returns></returns>        
        public static Queue<T>? ToQueue<T>(this IEnumerable<T> source)
        {
            if (source == null) return null;
            var queue = new Queue<T>(source);
            return queue;
        }

        /// <summary>
        /// Convert IEnumerable to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable? ToDataTable<T>(this IEnumerable<T> source)
        {
            if (source == null) return default;
            if (source is IEnumerable<dynamic> src) return ToDataTableForDynamic(src);
            var properties = GenericExtension.CompileGetter<T>();
            if (properties == null) return default;
            var typeName = typeof(T).TableNameAttributeValidate();
            var dt = new DataTable(typeName);
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
        private static DataTable? ToDataTableForDynamic(IEnumerable<dynamic> source)
        {
            var buffered = source.AsList();
            var firstElement = buffered.FirstOrDefault();
            if (firstElement == null) return default;
            if (firstElement is IDictionary<string, object> dict)
            {
                var dt = new DataTable("dynamicDt");
                var properties = dict.Keys;
                foreach (var property in properties)
                {
                    dt.Columns.Add(property);
                }
                foreach (IDictionary<string, object> item in buffered)
                {
                    var dr = dt.NewRow();
                    foreach (var property in properties)
                    {
                        dr[property] = item[property];
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return default;
        }
        /// <summary>
        /// Shuffle dataset inside source enumerable with each equally chance using Fisher-Yates-Durstenfeld shuffle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random = null)
        {
            if (source == null || !source.Any())
            {
                yield break;
            }
            var buffer = source.ToList();
            if (random == null)
                random = new Random();
            for (var idx = 0; idx < buffer.Count; idx++)
            {
                int tempIdx = random.Next(idx, buffer.Count);
                yield return buffer[tempIdx];
                buffer[tempIdx] = buffer[idx];
            }
        }
        /// <summary>
        /// Indicates whether the specified <see cref="IEnumerable{T}"/> is null or not contain any element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
    }
#if !NETSTANDARD2_1
    // These methods are available in System.Linq from .NET Core 3.0 and .NET Standard 2.1 onwards.
    public static partial class EnumerableExtension
    {
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
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return count <= 0 ? System.Linq.Enumerable.Empty<T>() : source.Skip(Math.Max(0, totalElement - count));
        }
        /// <summary>
        /// Appends a value to the end of the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="element">The value to append to source.</param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        {
            if (source.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var item in source)
            {
                yield return item;
            }
            yield return element;
        }
        /// <summary>
        /// Prepends a value to the start of the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="element">The value to append to source.</param>
        /// <returns></returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
        {
            if (source.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(source));
            }
            yield return element;
            foreach (var item in source)
            {
                yield return item;
            }
        }
        /// <summary>
        /// Take a sequence of elements from the start and skip N elements from the end.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            var totalElement = source.Count();
            if (totalElement < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return count <= 0 ? source : source.SubEnumerable(0, totalElement - count);
        }
    }
#endif
    #region mixed implementation
    public static partial class EnumerableExtension
    {
        /// Implementation of Dapper's AsList (https://github.com/StackExchange/Dapper/blob/master/Dapper/SqlMapper.cs) Licensed as http://www.apache.org/licenses/LICENSE-2.0
        /// <summary>
        /// Obtains the data as a list; if it is *already* a list, the original object is returned without
        /// any duplication; otherwise, ToList() is invoked.
        /// </summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The enumerable to return as a list.</param>
        public static List<T> AsList<T>(this IEnumerable<T> source) => (source == null || source is List<T>) ? (List<T>)source : source.ToList();
        /// <summary>
        /// Distinct the source data by prefer selector.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> source, Func<T, bool> selector)
        {
            return source.GroupBy(selector).Select(x => x.First());
        }
    }
    #endregion
}