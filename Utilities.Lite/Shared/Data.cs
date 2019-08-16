using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Utilities.Attributes.SQL;
using Utilities.Enumerables;

namespace Utilities.Shared
{
    /// <summary>
    /// This class contains a generic way to build data from specific source such as DbDataReader or from object itself
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// Convert DbDataReader into POCO object using reflection with unsafe operation (superior in speed but had no consistency guaruntee)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T RowBuilder<T>(this DbDataReader row) where T : new()
        {
            T instance = new T();
            foreach (var property in typeof(T).PropertiesBindingFlagsAttributeValidate())
            {
                try
                {
                    var value = row[property.Name];
                    if (value == DBNull.Value)
                    {
                        property.SetValue(instance, null);
                    }
                    else
                    {
                        property.SetValue(instance, value);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return instance;
        }
        /// <summary>
        /// Convert DbDataReader into POCO object using reflection using implicit inference (torelance for mismatch data type but slower down the building process)
        /// </summary>
        /// <typeparam name="T">typeof specific PO</typeparam>
        /// <param name="row">data reader to convert to POCO object</param>
        /// <returns></returns>
        public static T RowBuilderStrict<T>(this DbDataReader row) where T : new()
        {
            object instance = new T();
            foreach (var property in typeof(T).PropertiesBindingFlagsAttributeValidate())
            {
                try
                {
                    var propertyType = property.PropertyType;
                    var propertyName = property.Name;
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
            foreach (var property in typeof(T).PropertiesBindingFlagsAttributeValidate())
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
                    var name = property.FieldNameAttributeValidate();
                    values.Add(name, value ?? DBNull.Value);
                }
                catch
                {
                    continue;
                }
            }
            return values;
        }

        internal static IEnumerable<string> GenerateSQLCreteFieldStatement<T>()
            where T : class, new()
        {
            List<string> converter = new List<string>();
            foreach (var property in typeof(T).PropertiesBindingFlagsAttributeValidate())
            {
                try
                {
                    var propertyType = property.PropertyType;
                    var propertyName = AttributeExtension.FieldNameAttributeValidate(property);
                    var primaryKeyPostfix = property.IsSQLPrimaryKeyAttribute() ? " PRIMARY KEY " : "";
                    if (propertyType == typeof(string))
                    {
                        converter.Add($"{propertyName} NVARCHAR(1024) {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(char) || propertyType == typeof(char?))
                    {
                        converter.Add($"{propertyName} NCHAR(1) {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(short) || propertyType == typeof(short?) || propertyType == typeof(ushort) || propertyType == typeof(ushort?))
                    {
                        converter.Add($"{propertyName} SMALLINT {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(int) || propertyType == typeof(int?) || propertyType == typeof(uint) || propertyType == typeof(uint?))
                    {
                        converter.Add($"{propertyName} INT {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(long) || propertyType == typeof(long?) || propertyType == typeof(ulong) || propertyType == typeof(ulong?))
                    {
                        converter.Add($"{propertyName} BIGINT {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(float) || propertyType == typeof(float?))
                    {
                        converter.Add($"{propertyName} REAL {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(double) || propertyType == typeof(double?))
                    {
                        converter.Add($"{propertyName} FLOAT {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    {
                        converter.Add($"{propertyName} BIT {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                    {
                        converter.Add($"{propertyName} MONEY {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    {
                        converter.Add($"{propertyName} DATETIME {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        converter.Add($"{propertyName} UNIQUEIDENTIFIER {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(byte) || propertyType == typeof(byte?) || propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
                    {
                        converter.Add($"{propertyName} TINYINT {primaryKeyPostfix}");
                    }
                    else if (propertyType == typeof(byte[]))
                    {
                        converter.Add($"{propertyName} VARBINARY {primaryKeyPostfix}");
                    }
                }
                catch
                {
                    continue; //skip error property
                }
            }
            return converter;
        }
    }
}
