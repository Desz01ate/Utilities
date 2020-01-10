using System;
using System.Reflection;
using Utilities.Attributes.SQL;
using Utilities.Shared;

namespace Utilities.Classes
{
    /// <summary>
    /// Custom class which reflect the characteristic of <seealso cref="PropertyInfo"/> SetValue while maintain better memory footprint and execution speed.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public sealed class PropertySetterInfo<TSource>
    {
        private readonly Action<TSource, object> Setter;
        /// <summary>
        /// Gets the name of current member.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Gets the <seealso cref="FieldAttribute"/> value of current member.
        /// </summary>
        public readonly string FieldName;
        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public readonly Type PropertyType;
        /// <summary>
        /// Gets the index of this property.
        /// </summary>
        public readonly int PropertyIndex;
        internal PropertySetterInfo(PropertyInfo propertyInfo, int propertyIndex, Action<TSource, object> func)
        {
            Name = propertyInfo.Name;
            FieldName = AttributeExtension.FieldNameAttributeValidate(propertyInfo);
            PropertyType = propertyInfo.PropertyType;
            Setter = func;
            PropertyIndex = propertyIndex;
        }
        /// <summary>
        /// Sets the property value of a specified object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        public void SetValue(TSource source, object value) => Setter(source, value);
    }
}
