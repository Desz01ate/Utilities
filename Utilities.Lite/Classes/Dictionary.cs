using System.Collections.Generic;

namespace Utilities.Classes
{
    /// <summary>
    /// Represents a collection of multi-keys and values.
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Dictionary<TKey1, TKey2, TValue> : Dictionary<(TKey1, TKey2), TValue>
    {
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                return base[(key1, key2)];
            }
            set
            {
                base[(key1, key2)] = value;
            }
        }
        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            base.Add((key1, key2), value);
        }
        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return base.ContainsKey((key1, key2));
        }
    }
}
