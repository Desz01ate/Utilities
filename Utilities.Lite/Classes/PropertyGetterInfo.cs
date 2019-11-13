using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utilities.Classes
{
    public sealed class PropertyGetterInfo<TSource>
    {
        public PropertyInfo Info { get; }
        public Func<TSource, object> GetValue { get; }
        public string Name => Info?.Name;
        public string FieldName { get; }
        public Type PropertyType => Info?.PropertyType;
        internal PropertyGetterInfo(PropertyInfo propertyInfo, Func<TSource, object> func)
        {
            Info = propertyInfo;
            GetValue = func;
            FieldName = Utilities.Shared.AttributeExtension.FieldNameAttributeValidate(propertyInfo);
        }
    }
}
