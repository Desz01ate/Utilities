using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Classes
{
    /// <summary>
    /// Collection of DynamicObjectMetadata which describe the looks and shape of dictionary dynamic object.
    /// </summary>
    public class DynamicObjectAnalyzer : IEnumerable<DynamicObjectMetadata>
    {
        private Dictionary<string, DynamicObjectMetadata> Members { get; }
        /// <summary>
        /// Available object for dynamic object.
        /// </summary>
        public IEnumerable<object> Values => Members?.Select(x => x.Value.Value);
        /// <summary>
        /// Available keys for dynamic object.
        /// </summary>
        public IEnumerable<string> Keys => Members?.Select(x => x.Key);
        /// <summary>
        /// Get value by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DynamicObjectMetadata this[string key] => Members[key];
        /// <summary>
        /// Get value by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DynamicObjectMetadata this[int index] => Members.ElementAt(index).Value;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="obj"></param>
        public DynamicObjectAnalyzer(dynamic obj)
        {
            if (obj is IList<dynamic> list)
            {
                obj = list.FirstOrDefault();
            }
            if (obj is IDictionary<string, object> dict)
            {
                Members = new Dictionary<string, DynamicObjectMetadata>();
                ConstructMembers(dict);
            }

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dict"></param>
        public DynamicObjectAnalyzer(IDictionary<string, object> dict)
        {
            Members = new Dictionary<string, DynamicObjectMetadata>();
            ConstructMembers(dict);
        }
        private void ConstructMembers(IDictionary<string, object> dict)
        {
            foreach (var pair in dict)
            {
                var type = pair.Value?.GetType();
                var key = pair.Key;
                var value = pair.Value;
                this.Add(type, key, value, dict);
            }
        }
        private void Add(Type type, string key, object value, dynamic parent = null)
        {
            if (Members.ContainsKey(key)) throw new ArgumentException($"{key} is already exists.");
            var metadata = new DynamicObjectMetadata(type, key, value, parent);
            Members.Add(key, metadata);
        }
        /// <summary>
        /// Try parse dynamic object into specified object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryParse<T>(out T result) where T : new()
        {
            try
            {
                result = new T();
                var properties = Utilities.Shared.GenericExtension.CompileSetter<T>();
                foreach (var property in properties)
                {
                    var value = this[property.Name];
                    property.SetValue(result, value);
                }
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Parse dynamic object into specified object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Parse<T>() where T : new()
        {
            var result = new T();
            var properties = Utilities.Shared.GenericExtension.CompileSetter<T>();
            foreach (var property in properties)
            {
                var value = this[property.Name].Value;
                property.SetValue(result, value);
            }
            return result;
        }

        public IEnumerator<DynamicObjectMetadata> GetEnumerator()
        {
            if (Members == null)
            {
                yield break;
            }
            foreach (var m in Members)
            {
                yield return m.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void SaveModel(string filePath, string className = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            filePath = System.IO.Path.GetFullPath(filePath);
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"class {className ?? "Class1"}");
            sb.AppendLine(@"{");
            foreach (var member in Members)
            {
                sb.AppendLine($"    public {member.Value.Type} {member.Value.Name} {{ get; set; }}");
            }
            sb.AppendLine(@"}");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
    }

    /// <summary>
    /// Contains information of member of dynamic object.
    /// </summary>
    public class DynamicObjectMetadata : IEquatable<DynamicObjectMetadata>
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public object Value { get; private set; }
        public dynamic Parent { get; private set; }
        internal DynamicObjectMetadata(Type type, string key, object value, dynamic parent = null)
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

        public bool Equals(DynamicObjectMetadata other)
        {
            return this.Equals(other);
        }
    }
}