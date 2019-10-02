using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            if (primaryKeyProperty == null) throw new AttributeException(typeof(PrimaryKeyAttribute), type.FullName, null);
            if (primaryKeyProperty.Count() != 1) throw new InvalidMultipleAttributesException(typeof(PrimaryKeyAttribute), type.FullName, null);
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
        internal static IEnumerable<string> FieldNameAttributeValidate(this IEnumerable<PropertyInfo> propertyInfos)
        {
            foreach (var property in propertyInfos)
            {
                yield return FieldNameAttributeValidate(property);
            }
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
        private static Dictionary<Type, IEnumerable<PropertyInfo>> _propertiesCached = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        internal static PropertyInfo GetUnderlyingPropertyByName(this Type type, string propertyName)
        {
            IEnumerable<PropertyInfo> properties;
            if (_propertiesCached.TryGetValue(type, out var existingValue))
            {
                properties = existingValue;
            }
            else
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                _propertiesCached.Add(type, properties);
            }
            foreach (var property in properties)
            {
                var attrib = property.GetCustomAttribute<FieldAttribute>(true);
                string propName = attrib == null ? property.Name : attrib.FieldName;
                if (propName == propertyName) return property;
            }
            throw new Exception($"Property {propertyName} is not found on type '{type.FullName}");
        }
    }
}
