using System;
using System.Reflection;
using Utilities.Shared;

namespace Utilities.Classes
{
    public sealed class PropertySetterInfo<TSource>
    {
        private Action<TSource, object> Setter { get; }
        public string Name { get; }
        public string FieldName { get; }
        public Type PropertyType { get; }
        public int PropertyIndex { get; }
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
