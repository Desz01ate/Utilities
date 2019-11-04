using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Classes
{
    /// <summary>
    /// Collection of DynamicObjectMetadata.
    /// </summary>
    public class DynamicObjectMetadataCollection
    {
        private Dictionary<string, DynamicObjectMetadata> Members { get; } = new Dictionary<string, DynamicObjectMetadata>();

        /// <summary>
        /// Available object for dynamic object.
        /// </summary>
        public IEnumerable<object> Values => Members.Select(x => x.Value.Value);
        /// <summary>
        /// Available keys for dynamic object.
        /// </summary>
        public IEnumerable<string> Keys => Members.Select(x => x.Key);
        /// <summary>
        /// Get value by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (Members.ContainsKey(key))
                    return Members[key].Value;
                else
                    return null;
            }
        }
        internal void Add(Type type, string key, object value, dynamic parent = null)
        {
            if (Members.ContainsKey(key)) throw new ArgumentException($"{key} is already exists.");
            var metadata = new DynamicObjectMetadata(type, key, value, parent);
            Members.Add(key, metadata);
        }
        /// <summary>
        /// Try convert dynamic object into specified object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryConvertTo<T>(out T result) where T : class, new()
        {
            try
            {
                result = new T();
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var value = this[property.Name];
                    property.SetValue(result, value);
                }
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Contains information of member of dynamic object.
    /// </summary>
    public struct DynamicObjectMetadata
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public object Value { get; private set; }
        public dynamic Parent { get; private set; }
        public DynamicObjectMetadata(Type type, string key, object value, dynamic parent = null)
        {
            this.Name = key;
            this.Type = type;
            this.Value = value;
            this.Parent = parent;
        }

        public override bool Equals(object obj)
        {
            if (obj is DynamicObjectMetadata metadata)
            {
                return metadata.Name == this.Name && metadata.Type == this.Type && metadata.Value == this.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 31;
                hash = hash * 37 + this.Name.GetHashCode();
                hash = hash * 41 + this.Type.GetHashCode();
                hash = hash * 43 + this.Value.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(DynamicObjectMetadata left, DynamicObjectMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DynamicObjectMetadata left, DynamicObjectMetadata right)
        {
            return !(left == right);
        }
    }
}