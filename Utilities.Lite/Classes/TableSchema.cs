using System;

namespace Utilities.Classes
{
    public class TableSchema
    {
        public string ColumnName { get; set; }
        public string BaseColumnName { get; set; }
        public string BaseTableName { get; set; }
        public Type DataType { get; set; }
        public Type ProviderSpecificDataType { get; set; }
        public string DataTypeName { get; set; }

        public int ColumnOrdinal { get; set; }
        public int ColumnSize { get; set; }
        public short NumericPrecision { get; set; }
        public short NumericScale { get; set; }

        public int ProviderType { get; set; }
        public int NonVersionedProviderType { get; set; }

        public bool IsUnique { get; set; }
        public bool IsColumnSet { get; set; }
        public bool AllowDBNull { get; set; }
        public bool IsKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsRowVersion { get; set; }
        public bool IsLong { get; set; }
        public bool IsReadOnly { get; set; }
    }
}