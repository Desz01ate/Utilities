using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Legacy.Attributes.SQL
{
    /// <summary>
    /// Attribute which specified which property is a primary key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}
