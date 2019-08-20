using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.SQL.Generator
{
    public class TypeScriptGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public TypeScriptGenerator(string connectionString, string directory, string @namespace) : base(connectionString, directory, @namespace)
        {
        }

        protected override string GetNullableDataType(SqlColumn column)
        {
            var typets = DataTypeMapper(column);
            if (column.AllowDBNull)
            {
                return $"{typets} | null";
            }
            return typets;
        }
        protected override void GenerateCodeFile(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"class {table.Name.Replace("-", "")}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($"    {col} : {GetNullableDataType(column)};");
            }
            sb.AppendLine("}");
            var filePath = Path.Combine(Directory, $@"{table.Name}.ts");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        protected override string DataTypeMapper(SqlColumn sqlColumn)
        {
            switch (sqlColumn.DataTypeName)
            {
                case "bit":
                    return "boolean";

                case "tinyint":
                case "smallint":
                case "int":
                case "bigint":
                case "real":
                case "float":
                case "decimal":
                case "money":
                case "smallmoney":
                    return "number";

                case "time":
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "datetimeoffset":
                    return "Date";

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                    return "string";

                case "uniqueidentifier":
                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                case "variant":
                case "Udt":
                default:
                    // Fallback to be manually handled by user
                    return "any";
            };
        }
    }
}
