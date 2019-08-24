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
        internal InternalPropertyInfo ReferenceKeyProperty { get; }
        public ForeignKeyAttribute(Type referenceTable)
        {

            var primaryKeyOfReferenceTable = Shared.AttributeExtension.PrimaryKeyAttributeValidate(referenceTable);
            ReferenceKeyProperty = primaryKeyOfReferenceTable;

        }
    }
}
