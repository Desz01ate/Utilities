using System;

namespace Utilities.Attributes.SQL
{
    /// <summary>
    /// Attribute for table name customization
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}