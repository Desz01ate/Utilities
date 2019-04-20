﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Utilities.Shared
{
    public static class Data
    {
        public static T RowBuilder<T>(this SqlDataReader row, bool excludeBaseclass = true) where T : new()
        {
            object instance = new T();
            var type = typeof(T);
            var properties = type.GetProperties();
            if (excludeBaseclass)
            {
                properties = properties.Where(x => x.DeclaringType == type).ToArray();
            }
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var value = Convert.ToString(row[property.Name]);
                if (propertyType == typeof(string))
                {
                    property.SetValue(instance, value);
                }
                else if (propertyType == typeof(int) || propertyType == typeof(int?))
                {
                    property.SetValue(instance, Convert.ToInt32(value));
                }
                else if (propertyType == typeof(float) || propertyType == typeof(float?))
                {
                    property.SetValue(instance, Convert.ToSingle(value));
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
            }
            return (T)instance;
        }
        public static dynamic RowBuilder(this SqlDataReader row, IEnumerable<string> columns)
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
