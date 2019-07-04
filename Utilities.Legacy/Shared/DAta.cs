using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Utilities.Legacy.Attributes.SQL;
using Utilities.Legacy.Enumerables;

namespace Utilities.Legacy.Shared
{
    public static class Data
    {
        /// <summary>
        /// Convert DbDataReader into POCO object using reflection using implicit inference (torelance for mismatch data type but slower down the building process)
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
                    var propertyName = AttributeExtension.FieldNameValidate(property);
                    //this one generally slow down the overall performance compare to dynamic method but can
                    //safely sure that all value is going the right way
                    var value = Convert.ToString(row[propertyName]);
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
                    else if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
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
                    else if (propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
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
        /// Convert DbDataReader into POCO object using reflection using explicit inference (can cause the property to be default or null if mismatch data type but has a significant improved speed compare to implicitly style.)
        /// </summary>
        /// <typeparam name="T">typeof specific PO</typeparam>
        /// <param name="row">data reader to convert to POCO object</param>
        /// <returns></returns>
        public static T RowBuilderExplicit<T>(this DbDataReader row) where T : new()
        {
            object instance = new T();
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    var propertyType = property.PropertyType;
                    var propertyName = AttributeExtension.FieldNameValidate(property);
                    //this one generally slow down the overall performance compare to dynamic method but can
                    //safely sure that all value is going the right way
                    //if the value is not in string form, is can caused errors if the model is mismatch from the database
                    var value = row[propertyName];
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
                    else if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
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
                        property.SetValue(instance, DateTime.Parse(value.ToString()));
                    }
                    else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        property.SetValue(instance, Guid.Parse(value.ToString()));
                    }
                    else if (propertyType == typeof(byte) || propertyType == typeof(byte?))
                    {
                        property.SetValue(instance, Convert.ToByte(value));
                    }
                    else if (propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
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
                rowInstance.Add(column, row[column]);
            }
            return rowInstance;
        }



        internal static Dictionary<string, object> CRUDDataMapping<T>(T obj, SqlType type)
            where T : class, new()
        {
            var values = new Dictionary<string, object>();
            foreach (var property in typeof(T).GetProperties())
            {
                try
                {
                    var pIns = property.GetCustomAttribute<IgnoreFieldAttribute>(true);
                    if (pIns != null)
                    {
                        if ((pIns.IgnoreInsert && type == SqlType.Insert) || (pIns.IgnoreUpdate && type == SqlType.Update))
                        {
                            continue;
                        }
                    }
                    var value = property.GetValue(obj);
                    var name = property.FieldNameValidate();
                    values.Add(name, values == null ? DBNull.Value : value);
                }
                catch
                {
                    continue;
                }
                //try
                //{
                //    var propertyType = property.PropertyType;
                //    //this one generally slow down the overall performance compare to dynamic method but can
                //    //safely sure that all value is going the right way
                //    var value = property.GetValue(obj);
                //    if (propertyType == typeof(string)
                //        || propertyType == typeof(char) || propertyType == typeof(char?)
                //        || propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                //    {
                //        //values.Add($"'{value}'");
                //        values.Add($"@{property.Name}", value);
                //    }

                //    else if (propertyType == typeof(short) || propertyType == typeof(short?)
                //        || propertyType == typeof(int) || propertyType == typeof(int?)
                //        || propertyType == typeof(long) || propertyType == typeof(long?)
                //        || propertyType == typeof(float) || propertyType == typeof(float?)
                //        || propertyType == typeof(double) || propertyType == typeof(double?)
                //        || propertyType == typeof(ushort) || propertyType == typeof(ushort?)
                //        || propertyType == typeof(uint) || propertyType == typeof(uint?)
                //        || propertyType == typeof(ulong) || propertyType == typeof(ulong?)
                //        || propertyType == typeof(decimal) || propertyType == typeof(decimal?)
                //        || propertyType == typeof(byte) || propertyType == typeof(byte?)
                //        || propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
                //    {
                //        //values.Add($"{value}");
                //        values.Add($"@{property.Name}", value);

                //    }
                //    else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                //    {
                //        var v = (bool)value ? 1 : 0;
                //        //values.Add($"{value}");
                //        values.Add($"@{property.Name}", value);

                //    }
                //    else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                //    {
                //        var v = ((DateTime)value).ToString();
                //        //values.Add($"'{value}'");
                //        values.Add($"@{property.Name}", value);
                //    }
            }
            return values;
        }
        internal static Dictionary<string, object> UpdateDataMapping<T>(T obj)
            where T : class, new()
        {
            var values = new Dictionary<string, object>();
            var properties = (typeof(T).GetProperties()).ToList();
            properties.Remove(AttributeExtension.PrimaryKeyValidate(properties));
            foreach (var property in properties)
            {
                try
                {
                    var propertyType = property.PropertyType;
                    //this one generally slow down the overall performance compare to dynamic method but can
                    //safely sure that all value is going the right way
                    var value = property.GetValue(obj);
                    if (propertyType == typeof(string)
                        || propertyType == typeof(char) || propertyType == typeof(char?)
                        || propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        values.Add(property.Name, value);
                    }

                    else if (propertyType == typeof(short) || propertyType == typeof(short?)
                        || propertyType == typeof(int) || propertyType == typeof(int?)
                        || propertyType == typeof(long) || propertyType == typeof(long?)
                        || propertyType == typeof(float) || propertyType == typeof(float?)
                        || propertyType == typeof(double) || propertyType == typeof(double?)
                        || propertyType == typeof(ushort) || propertyType == typeof(ushort?)
                        || propertyType == typeof(uint) || propertyType == typeof(uint?)
                        || propertyType == typeof(ulong) || propertyType == typeof(ulong?)
                        || propertyType == typeof(decimal) || propertyType == typeof(decimal?)
                        || propertyType == typeof(byte) || propertyType == typeof(byte?)
                        || propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
                    {
                        //values.Add($"[{property.Name}] = {value}");
                        values.Add(property.Name, value);
                    }
                    else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    {
                        var v = (bool)value ? 1 : 0;
                        //values.Add($"[{property.Name}] = {value}");
                        values.Add(property.Name, value);
                    }
                    else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    {
                        var v = ((DateTime)value).ToString();
                        //values.Add($"[{property.Name}] = '{value}'");
                        values.Add(property.Name, value);
                    }
                }
                catch
                {
                    continue; //skip error property
                }
            }
            return values;
        }
        internal static string PrimaryKeyFormatter<T>(T obj, Type type)
        {
            try
            {
                var value = Convert.ToString(obj);
                if (type == typeof(string)
                    || type == typeof(char) || type == typeof(char?)
                    || type == typeof(Guid) || type == typeof(Guid?))
                {
                    return $"'{value}'";
                }

                else if (type == typeof(short) || type == typeof(short?)
                    || type == typeof(int) || type == typeof(int?)
                    || type == typeof(long) || type == typeof(long?)
                    || type == typeof(float) || type == typeof(float?)
                    || type == typeof(double) || type == typeof(double?)
                    || type == typeof(ushort) || type == typeof(ushort?)
                    || type == typeof(uint) || type == typeof(uint?)
                    || type == typeof(ulong) || type == typeof(ulong?)
                    || type == typeof(decimal) || type == typeof(decimal?)
                    || type == typeof(byte) || type == typeof(byte?)
                    || type == typeof(sbyte) || type == typeof(sbyte?))
                {
                    return $"{value}";
                }
                else if (type == typeof(bool) || type == typeof(bool?))
                {
                    var v = Convert.ToBoolean(value) ? 1 : 0;
                    return $"{value}";
                }
                else if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    var v = (DateTime.Parse(value)).ToString();
                    return $"'{value}'";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            throw new Exception($"Invalid primary key mapping for data type {typeof(T).Name}");
        }
    }
}
