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
    public sealed class CircularQueue<T> : IEnumerable<T>
    {
        private readonly ConcurrentQueue<T> q;
        private readonly int limit;
        public CircularQueue(int limitSize)
        {
            if (limitSize <= 0) throw new ArgumentOutOfRangeException(nameof(limitSize));
            limit = limitSize;
            q = new ConcurrentQueue<T>(new T[limit]);
        }
        public CircularQueue(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            limit = source.Count();
            q = new ConcurrentQueue<T>(source);
        }
        /// <summary>
        /// Enqueue element into the last of sequence.
        /// </summary>
        /// <param name="element">data to enqueue</param>
        public T Enqueue(T element)
        {
            T result = default;
            if (q.Count >= limit)
            {
                q.TryDequeue(out result);
            }
            q.Enqueue(element);
            return result;
        }
        /// <summary>
        /// Dequeue element from the start of the sequence.
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (q.TryDequeue(out var result))
            {
                return result;
            }
            return default;
        }
        /// <summary>
        /// return an object from the beginning of the queue
        /// without removing it.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (q.TryPeek(out var result))
            {
                return result;
            }
            return default;
        }
        /// <summary>
        /// Shift all elements by 1 index to the left.
        /// </summary>
        public void ShiftLeft()
        {
            if (q.TryDequeue(out var element))
            {
                q.Enqueue(element);
            }
        }
        /// <summary>
        /// Shift all elements by 1 index to the right.
        /// </summary>
        public void ShiftRight()
        {
            for (var i = 0; i < limit - 1; i++)
            {
                ShiftLeft();
            }
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
