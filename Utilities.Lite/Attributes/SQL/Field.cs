using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Attributes.SQL
{
    /// <summary>
    /// Attribute for field name customization
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class FieldAttribute : Attribute
    {
        public string FieldName { get; }
        public FieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
