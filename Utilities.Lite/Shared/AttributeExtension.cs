using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utilities.Attributes.SQL;
using Utilities.Classes;
using Utilities.Exceptions;

namespace Utilities.Shared
{
    internal static class AttributeExtension
    {
        internal static InternalPropertyInfo PrimaryKeyAttributeValidate(this Type type)
        {
            var properties = type.GetProperties();
            return PrimaryKeyAttributeValidate(properties, type);
        }
        internal static InternalPropertyInfo PrimaryKeyAttributeValidate(this IEnumerable<PropertyInfo> properties, Type type)
        {
            var primaryKeyProperty = properties.Where(property => property.GetCustomAttribute<PrimaryKeyAttribute>(true) != null);
            if (primaryKeyProperty == null) throw new AttributeException("PrimaryKey", type.FullName);
            if (primaryKeyProperty.Count() != 1) throw new InvalidMultipleAttributesException("PrimaryKey", type.FullName);
            var property = new InternalPropertyInfo(primaryKeyProperty.First());
            return property;
        }
        internal static bool IsSQLPrimaryKeyAttribute(this PropertyInfo property)
        {
            var attrib = property.GetCustomAttribute<PrimaryKeyAttribute>(true);
            return attrib != null;
        }
        internal static string FieldNameAttributeValidate(this PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<FieldAttribute>(true);
            if (attribute == null) return propertyInfo.Name;
            return attribute.FieldName;
        }
        internal static string TableNameAttributeValidate(this Type type)
        {
            var attribute = type.GetCustomAttribute<TableAttribute>(true);
            if (attribute == null) return type.Name;
            return attribute.TableName;
        }
        internal static bool NotNullAttributeValidate(this PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<NotNullAttribute>();
            return attribute != null;
        }
        internal static IEnumerable<InternalPropertyInfo> PropertiesBindingFlagsAttributeValidate(this Type type)
        {
            var attribute = type.GetCustomAttribute<BindingFlagsAttribute>(true);
            PropertyInfo[] properties = null;
            if (attribute == null)
            {
                properties = type.GetProperties();
            }
            else
            {
                properties = type.GetProperties(attribute.BindingFlags);
            }
            var internalProperties = properties.Select(property =>
            {
                var internalPropertyInfo = new InternalPropertyInfo(property);
                return internalPropertyInfo;
            });
            return internalProperties;
        }
        internal static IEnumerable<InternalPropertyInfo> ForeignKeyAttributeValidate(this Type type)
        {
            List<InternalPropertyInfo> properties = new List<InternalPropertyInfo>();

            foreach (var property in type.GetProperties())
            {
                var attribute = property.GetCustomAttribute<ForeignKeyAttribute>(true);
                if (attribute != null)
                {
                    //foreach (var p in attribute.ReferenceKeyProperty)
                    //{
                    //    p.ForeignKeyName = property.Name;
                    //    properties.Add(p);
                    //}
                    var refKey = attribute.ReferenceKeyProperty;
                    refKey.ForeignKeyName = property.Name;
                    properties.Add(refKey);

                }
            }
            return properties;
        }
    }
}
