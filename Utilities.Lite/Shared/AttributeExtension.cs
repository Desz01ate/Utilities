using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities.Attributes.SQL;
using Utilities.Classes;

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
            if (primaryKeyProperty == null) throw new Exception($"Can't find attribute [{typeof(PrimaryKeyAttribute).FullName}] in {type.FullName}.");
            if (primaryKeyProperty.Count() != 1) throw new Exception($"The attribute [{typeof(PrimaryKeyAttribute).FullName}] must specific one and only one. (error in {type.FullName} class)");
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
            PropertyInfo[] properties;
            if (attribute == null)
            {
                properties = type.GetProperties();
            }
            else
            {
                properties = type.GetProperties(attribute.BindingFlags);
            }
            foreach (var property in properties)
            {
                yield return new InternalPropertyInfo(property);
            }
        }
        internal static IEnumerable<InternalPropertyInfo> ForeignKeyAttributeValidate(this Type type)
        {
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
                    refKey.ForeignKeyName = property.FieldNameAttributeValidate();
                    yield return refKey;
                }
            }
        }
        //private static Dictionary<Type, PropertyInfo[]> _propertiesCached = new Dictionary<Type, PropertyInfo[]>();
        //internal static PropertyInfo GetUnderlyingPropertyByName(this Type type, string propertyName)
        //{
        //    PropertyInfo[] properties;
        //    if (_propertiesCached.TryGetValue(type, out var existingValue))
        //    {
        //        properties = existingValue;
        //    }
        //    else
        //    {
        //        properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //        _propertiesCached.Add(type, properties);
        //    }
        //    foreach (var property in properties)
        //    {
        //        var attrib = property.GetCustomAttribute<FieldAttribute>(true);
        //        string propName = attrib == null ? property.Name : attrib.FieldName;
        //        if (propName == propertyName) return property;
        //    }
        //    return null;
        //    //throw new Exception($"Property {propertyName} is not found on type '{type.FullName}");
        //}
        private static Dictionary<PropertyInfo, FieldAttribute> _attributesCached = new Dictionary<PropertyInfo, FieldAttribute>();
        internal static PropertyInfo GetUnderlyingPropertyByName(PropertyInfo[] properties, string propertyName)
        {
            foreach (var property in properties)
            {
                FieldAttribute attrib;
                if (_attributesCached.TryGetValue(property, out var atb))
                {
                    attrib = atb;
                }
                else
                {
                    attrib = property.GetCustomAttribute<FieldAttribute>(true);
                    _attributesCached.Add(property, attrib);
                }
                string propName = attrib == null ? property.Name : attrib.FieldName;
                if (propName == propertyName) return property;
            }
            return null;
        }
    }
}
