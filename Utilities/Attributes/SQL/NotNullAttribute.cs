using System;

namespace Utilities.Attributes.SQL
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : Attribute
    {
    }
}