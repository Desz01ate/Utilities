﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Utilities.Attributes.SQL;
using Utilities.Enum;
using Utilities.Interfaces;

namespace Utilities.Shared
{
    /// <summary>
    /// This class contains a generic way to build data from specific source such as DbDataReader or from object itself
    /// </summary>
    public static class DataExtension
    {
        /// <summary>
        /// Convert DataRow into dynamic object with specified column name.
        /// </summary>
        /// <param name="row">data reader to convert to dynamic object</param>
        /// <param name="columns">column name container</param>
        /// <returns></returns>
        public static dynamic RowBuilder(this DataRow row, IEnumerable<string> columns)
        {
            var rowInstance = new ExpandoObject() as IDictionary<string, object>;
            for (var idx = 0; idx < columns.Count(); idx++)
            {
                rowInstance.Add(columns.ElementAt(idx), row?[idx]);
            }
            return rowInstance;
        }
        /// <summary>
        /// Convert IDataReader into dynamic object.
        /// </summary>
        /// <param name="row">data reader to convert to dynamic object</param>
        /// <returns></returns>
        public static dynamic RowBuilder(this IDataReader row)
        {
            var rowInstance = new ExpandoObject() as IDictionary<string, object>;
            for (var idx = 0; idx < row.FieldCount; idx++)
            {
                rowInstance.Add(row.GetName(idx), row[idx]);
            }
            return rowInstance;
        }
        /// <summary>
        /// Convert DataRow into dynamic object.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static dynamic RowBuilder(this DataRow row)
        {
            var rowInstance = new ExpandoObject() as IDictionary<string, object>;
            var columns = row.Table.Columns;
            foreach (DataColumn column in columns)
            {
                rowInstance.Add(column.ColumnName, row?[column]);
            }
            return rowInstance;
        }
        /// <summary>
        /// Convert data row into POCO.
        /// </summary>
        /// <typeparam name="T">Class that implement IExcelReader</typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ConvertDataRowTo<T>(DataRow dr) where T : IExcelReader, new()
        {
            var properties = typeof(T).GetProperties();
            var obj = new T();

            for (var idx = 0; idx < properties.Length; idx++)
            {
                var property = properties[idx];
                var externalIndex = obj.GetExternalColumnIndex(property.Name);
                var value = dr?[externalIndex];
                property.SetValue(obj, value);
            }
            return obj;
        }

        internal static Dictionary<string, object> CRUDDataMapping<T>(T obj, SqlType type)
            where T : class, new()
        {
            var values = new Dictionary<string, object>();
            foreach (var property in typeof(T).PropertiesBindingFlagsAttributeValidate())
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
            return values;
        }

        internal static IEnumerable<string> GenerateSQLCreteFieldStatement<T1, T2, T>(this IDatabaseConnectorExtension<T1, T2> connector)
            where T : class, new()
            where T1 : DbConnection, new()
            where T2 : DbParameter, new()
        {
            var properties = typeof(T).PropertiesBindingFlagsAttributeValidate();
            var referenceProperties = typeof(T).ForeignKeyAttributeValidate();
            foreach (var property in properties)
            {
                var propertyName = AttributeExtension.FieldNameAttributeValidate(property);
                var IsNotNull = AttributeExtension.NotNullAttributeValidate(property);
                var primaryKeyPostfix = property.IsSQLPrimaryKeyAttribute() ? " PRIMARY KEY " : "";
                var notNullPostfix = IsNotNull ? " NOT NULL " : "";
                var sqlType = connector.MapCLRTypeToSQLType(property.PropertyType);
                yield return ($"{propertyName} {sqlType} {primaryKeyPostfix} {notNullPostfix}");
            }
            foreach (var foreignKey in referenceProperties)
            {
                var propertyName = AttributeExtension.FieldNameAttributeValidate(foreignKey);
                var targetTable = foreignKey.DeclaringType.TableNameAttributeValidate();
                yield return ($"CONSTRAINT fk_{typeof(T).Name}_{targetTable} FOREIGN KEY ({foreignKey.ForeignKeyName}) REFERENCES {targetTable} ({propertyName})");
            }
        }
    }
}