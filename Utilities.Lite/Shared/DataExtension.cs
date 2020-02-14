using System;
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
    internal static class DataExtension
    {
        /// <summary>
        /// Convert IDataReader into dynamic object.
        /// </summary>
        /// <param name="row">data reader to convert to dynamic object</param>
        /// <returns></returns>
        internal static dynamic RowBuilder(this IDataReader row)
        {
            var rowInstance = new ExpandoObject() as IDictionary<string, object>;
            for (var idx = 0; idx < row.FieldCount; idx++)
            {
                rowInstance.Add(row.GetName(idx), row[idx]);
            }
            return rowInstance;
        }

        internal static Dictionary<string, object> InsertUpdateMapper<T>(T obj, SqlType type)
            where T : class
        {
            var values = new Dictionary<string, object>();
            foreach (var property in typeof(T).PropertiesBindingFlagsAttributeValidate())
            {
                var primaryKey = property.GetCustomAttribute<PrimaryKeyAttribute>(true);
                var isPrimaryKey = primaryKey != null;
                var ignoreField = property.GetCustomAttribute<IgnoreFieldAttribute>(true);
                var isIgnoreField = ignoreField != null;
                if (isIgnoreField || isPrimaryKey)
                {
                    var disallowInsert = (isIgnoreField && ignoreField.IgnoreInsert) || (isPrimaryKey && primaryKey.AutoIncrement);
                    var disallowUpdate = (isIgnoreField && ignoreField.IgnoreUpdate);
                    if (   type == SqlType.Insert && disallowInsert 
                        || type == SqlType.Update && disallowUpdate)
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
    }
}