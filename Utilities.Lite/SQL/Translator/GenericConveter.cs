using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Utilities.Classes;
using Utilities.Shared;
using Utilities.SQL.Interfaces;

namespace Utilities.SQL.Translator
{
    internal sealed class GenericConverter<T> : IDataMapper<T> where T : new()
    {
        private readonly DbDataReader _Reader;
        private readonly List<PropertySetterInfo<T>> _Properties = new List<PropertySetterInfo<T>>();
        public GenericConverter(DbDataReader reader)
        {
            _Reader = reader;
            var allowedColumns = reader.GetColumns();
            var allowedProperties = GenericExtension.CompileSetter<T>();
            foreach (var property in allowedProperties)
            {
                if (allowedColumns.Contains(property.FieldName))
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
                var value = _Reader[property.FieldName];
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
            while (this._Reader.Read())
            {
                yield return Mapper();
            }
        }
#if NETSTANDARD2_1
        public async IAsyncEnumerable<T> GenerateObjectsAsync()
        {
            while (await this._Reader.ReadAsync())
            {
                yield return Mapper();
            }
        }
#endif
    }
}