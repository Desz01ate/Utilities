using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utilities.Classes;

namespace Utilities.Attributes.SQL
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ForeignKeyAttribute : Attribute
    {
        internal List<InternalPropertyInfo> ReferenceKeyProperty { get; }
        public ForeignKeyAttribute(params Type[] referenceTable)
        {
            ReferenceKeyProperty = new List<InternalPropertyInfo>();
            foreach (var table in referenceTable)
            {
                var primaryKeyOfReferenceTable = Shared.AttributeExtension.PrimaryKeyAttributeValidate(table);
                ReferenceKeyProperty.Add(primaryKeyOfReferenceTable);
            }
        }
    }
}
