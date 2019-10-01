namespace Utilities.Enum
{
    /// <summary>
    /// Provide schema restrictions type for SQL Server (https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/schema-restrictions)
    /// </summary>
    public enum SchemaRestriction
    {
        Users,
        Databases,
        Tables,
        Columns,
        StructuredTypeMembers,
        Views,
        ViewColumns,
        ProcedureParameters,
        Procedures,
        IndexColums,
        Indexes,
        UserDefinedTypes,
        ForeignKeys,
    }
}