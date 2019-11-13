using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utilities.Classes
{
    public sealed class PropertySetterInfo<TSource>
    {
        public Action<TSource, object> SetValue { get; }
        public string Name { get; }
        public string FieldName { get; internal set; }
        public Type PropertyType { get; }
        public int PropertyIndex { get; }
        internal PropertySetterInfo(PropertyInfo propertyInfo, int propertyIndex, Action<TSource, object> func)
        {
            Name = propertyInfo.Name;
            PropertyType = propertyInfo.PropertyType;
            SetValue = func;
            PropertyIndex = propertyIndex;
        }
    }
}
