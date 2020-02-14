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

        internal static Dictionary<string, object> CRUDDataMapping<T>(T obj, SqlType type)
            where T : class
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
    }
}