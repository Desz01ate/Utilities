using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;

namespace Utilities.Shared
{
    public static class Data
    {
        /// <summary>
        /// Convert DbDataReader into POCO object using reflecting
        /// </summary>
        /// <typeparam name="T">typeof specific PO</typeparam>
        /// <param name="row">data reader to convert to POCO object</param>
        /// <returns></returns>
        public static T RowBuilder<T>(this DbDataReader row) where T : new()
        {
            object instance = new T();
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    var propertyType = property.PropertyType;
                    //this one generally slow down the overall performance compare to dynamic method but can
                    //safely sure that all value is going the right way
                    var value = Convert.ToString(row[property.Name]);
                    if (propertyType == typeof(string))
                    {
                        property.SetValue(instance, value);
                    }
                    else if (propertyType == typeof(char) || propertyType == typeof(char?))
                    {
                        property.SetValue(instance, Convert.ToChar(value));
                    }
                    else if (propertyType == typeof(short) || propertyType == typeof(short?))
                    {
                        property.SetValue(instance, Convert.ToInt16(value));
                    }
                    else if (propertyType == typeof(int) || propertyType == typeof(int?))
                    {
                        property.SetValue(instance, Convert.ToInt32(value));
                    }
                    else if (propertyType == typeof(long) || propertyType == typeof(long?))
                    {
                        property.SetValue(instance, Convert.ToInt64(value));
                    }
                    else if (propertyType == typeof(float) || propertyType == typeof(float?))
                    {
                        property.SetValue(instance, Convert.ToSingle(value));
                    }
                    else if (propertyType == typeof(double) || propertyType == typeof(double?))
                    {
                        property.SetValue(instance, Convert.ToDouble(value));
                    }
                    else if (propertyType == typeof(ushort) || propertyType == typeof(ushort?))
                    {
                        property.SetValue(instance, Convert.ToUInt16(value));
                    }
                    else if (propertyType == typeof(uint) || propertyType == typeof(uint?))
                    {
                        property.SetValue(instance, Convert.ToUInt32(value));
                    }
                    else if (propertyType == typeof(ulong) || propertyType == typeof(ulong))
                    {
                        property.SetValue(instance, Convert.ToUInt64(value));
                    }
                    else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    {
                        property.SetValue(instance, Convert.ToBoolean(value));
                    }
                    else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                    {
                        property.SetValue(instance, Convert.ToDecimal(value));
                    }
                    else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    {
                        property.SetValue(instance, DateTime.Parse(value));
                    }
                    else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        property.SetValue(instance, Guid.Parse(value));
                    }
                    else if (propertyType == typeof(byte) || propertyType == typeof(byte?))
                    {
                        property.SetValue(instance, Convert.ToByte(value));
                    }
                    else if (propertyType == typeof(sbyte) || propertyType == typeof(sbyte))
                    {
                        property.SetValue(instance, Convert.ToSByte(value));
                    }
                }
                catch
                {
                    continue; //skip error property
                }
            }
            return (T)instance;
        }
        /// <summary>
        /// Convert DbDataReader into dynamic object with specified column name
        /// </summary>
        /// <param name="row">data reader to convert to dynamic object</param>
        /// <param name="columns">column name container</param>
        /// <returns></returns>
        public static dynamic RowBuilder(this DbDataReader row, IEnumerable<string> columns)
        {
            var rowInstance = new ExpandoObject() as IDictionary<string, object>;
            foreach (var column in columns)
            {
                rowInstance.Add(column, row[column].ToString());
            }
            return rowInstance;
        }

    }
}
