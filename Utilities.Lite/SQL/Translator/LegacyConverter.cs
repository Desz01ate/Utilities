using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using Utilities.Classes;
using Utilities.Shared;
using Utilities.SQL.Interfaces;

namespace Utilities.SQL.Translator
{
    /// <summary>
    /// Legacy and no-optimize mapper using direct PropertyInfo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LegacyConverter<T> : IDataMapper<T> where T : new()
    {
        private readonly DbDataReader _Reader;
        //private readonly PropertySetterInfo<T>[] _Properties;
        private readonly List<PropertyInfo> _Properties = new List<PropertyInfo>();
        public LegacyConverter(DbDataReader reader)
        {
            _Reader = reader;
            var allowedColumns = reader.GetColumns();
            var allowedProperties = typeof(T).GetProperties();
            foreach (var property in allowedProperties)
            {
                var propName = property.FieldNameAttributeValidate();
                if (allowedColumns.Contains(propName))
                {
                    _Properties.Add(property);
                }
            }
        }
        private T Mapper()
        {
            var obj = new T();
            foreach (var property in _Properties)
            {
                var value = _Reader[property.Name];
                if (value == DBNull.Value) continue;
                property.SetValue(obj, value);
            }
            return obj;
        }
        public T GenerateObject()
        {
            return Mapper();
        }

        public IEnumerable<T> GenerateObjects()
        {
            while (_Reader.Read())
            {
                yield return Mapper();
            }
        }
    }
}
