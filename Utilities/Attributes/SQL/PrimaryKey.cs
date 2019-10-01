using System;

namespace Utilities.Attributes.SQL
{
    /// <summary>
    /// Attribute which specified which property is a primary key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}