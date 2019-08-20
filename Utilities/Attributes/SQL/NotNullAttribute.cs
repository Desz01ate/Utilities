using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Attributes.SQL
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : Attribute
    {
    }
}
