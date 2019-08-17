using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utilities.Attributes.SQL;
using Utilities.Exceptions;

namespace Utilities.Shared
{
    internal static class AttributeExtension
    {
        internal static PropertyInfo PrimaryKeyAttributeValidate(this Type t)
        {
            var properties = t.GetProperties();
            return PrimaryKeyAttributeValidate(properties);
        }
        internal static PropertyInfo PrimaryKeyAttributeValidate(this IEnumerable<PropertyInfo> properties)
        {
            var primaryKeyProperty = properties.Where(property =>
            {
                var attrib = property.GetCustomAttribute<PrimaryKeyAttribute>(true);
                if (attrib != null) return true;
                return false;
            });
            if (primaryKeyProperty == null) throw new AttributeException("PrimaryKey");
            if (primaryKeyProperty.Count() != 1) throw new InvalidMultipleAttributesException("PrimaryKey");
            return primaryKeyProperty.First();
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
        internal static IEnumerable<PropertyInfo> PropertiesBindingFlagsAttributeValidate(this Type type)
        {
            var attribute = type.GetCustomAttribute<BindingFlagsAttribute>(true);
            if (attribute == null) return type.GetProperties();
            return type.GetProperties(attribute.BindingFlags);
        }
    }
}
