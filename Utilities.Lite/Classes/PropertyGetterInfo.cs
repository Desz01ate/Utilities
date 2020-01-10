using System;
using System.Reflection;
using Utilities.Attributes.SQL;

namespace Utilities.Classes
{
    /// <summary>
    /// Custom class which reflect the characteristic of <seealso cref="PropertyInfo"/> GetValue while maintain better memory footprint and execution speed.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public sealed class PropertyGetterInfo<TSource>
    {
        private Func<TSource, object> Getter { get; }
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
        internal PropertyGetterInfo(PropertyInfo propertyInfo, Func<TSource, object> func)
        {
            Name = propertyInfo.Name;
            FieldName = Shared.AttributeExtension.FieldNameAttributeValidate(propertyInfo);
            PropertyType = propertyInfo.PropertyType;
            Getter = func;
        }
        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public object GetValue(TSource source) => Getter(source);
    }
}
