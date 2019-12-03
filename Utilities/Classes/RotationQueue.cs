using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Classes
{
    /// <summary>
    /// Fixed-size queue which automatically dequeue when elements are going to exceed the limit.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class RotationQueue<T> : IEnumerable<T>
    {
        private readonly ConcurrentQueue<T> q;
        private readonly int limit;
        public RotationQueue(int limitedSize)
        {
            if (limitedSize <= 0) throw new ArgumentOutOfRangeException("Size must between 1 to 255");
            limit = limitedSize;
            q = new ConcurrentQueue<T>(new T[limit]);
        }
        public RotationQueue(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            limit = source.Count();
            q = new ConcurrentQueue<T>(source);
        }
        /// <summary>
        /// Enqueue object into the last of sequence.
        /// </summary>
        /// <param name="obj"></param>
        public void Enqueue(T obj)
        {
            if (q.Count >= limit)
            {
                q.TryDequeue(out var _);
            }
            q.Enqueue(obj);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var data in q)
            {
                yield return data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
