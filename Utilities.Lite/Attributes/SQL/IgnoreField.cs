using System;

namespace Utilities.Attributes.SQL
{
    /// <summary>
    /// Attribute for ignorance on insert or update statement
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class IgnoreFieldAttribute : Attribute
    {
        public bool IgnoreInsert { get; }
        public bool IgnoreUpdate { get; }

        public IgnoreFieldAttribute(bool insert, bool update)
        {
            IgnoreInsert = insert;
            IgnoreUpdate = update;
        }
    }
}