using ModelGenerator.Services.Generator.Model;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator
{
    public class VisualBasicGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public VisualBasicGenerator(string connectionString, string directory, string @namespace, Func<string, string> func = null) : base(connectionString, directory, @namespace)
        {
            if (func != null) this.SetCleanser(func);
        }

        protected override string GetNullableDataType(TableSchema column)
        {
            var typevb = DataTypeMapper(column.DataTypeName);
            if (column.AllowDBNull && typevb != "String")
            {
                return $"Nullable(Of {typevb})";
            }
            return typevb;
        }
        protected override void GenerateCodeFile(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Imports System");
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                sb.AppendLine($@"Namespace {Namespace}");
            }
            sb.AppendLine("'You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite");
            sb.AppendLine($"'<Utilities.Attributes.SQL.Table(\"[{table.Name}]\")>");
            sb.AppendLine($@"Public Class {table.Name.Replace("-", "")}");
            foreach (var column in table.Columns)
            {
                sb.AppendLine();
                var type = Utilities.String.ToLeadingUpper(GetNullableDataType(column));
                var col = ColumnNameCleanser(column.ColumnName);

                sb.AppendLine($"    Private _{col} As {type}");
                sb.AppendLine();
                if (column.ColumnName == table.PrimaryKey)
                {
                    sb.AppendLine($"    '<Utilities.Attributes.SQL.PrimaryKey>");
                }
                sb.AppendLine($"    Public Property [{col}] As {type}");
                sb.AppendLine($"      Get");
                sb.AppendLine($"         Return _{col}");
                sb.AppendLine($"      End Get");
                sb.AppendLine($"      Set(value As {type})");
                sb.AppendLine($"         _{col} = value");
                sb.AppendLine($"      End Set");
                sb.AppendLine($"    End Property");
            }
            sb.AppendLine();
            sb.AppendLine("End Class");
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                sb.AppendLine($@"End Namespace");
            }
            var filePath = Path.Combine(Directory, $@"{table.Name}.vb");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        protected override string DataTypeMapper(string columnType)
        {
            switch (columnType)
            {
                case "bit":
                    return "Boolean";

                case "tinyint":
                    return "Byte";
                case "smallint":
                    return "Short";
                case "int":
                    return "Integer";
                case "bigint":
                    return "Long";

                case "real":
                    return "Single";
                case "float":
                    return "Double";
                case "decimal":
                case "money":
                case "smallmoney":
                    return "Decimal";

                case "time":
                    return "TimeSpan";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "Date";
                case "datetimeoffset":
                    return "Date";

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                    return "String";

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                    return "Byte";

                case "uniqueidentifier":
                    return "Guid";

                case "variant":
                case "Udt":
                    return "Object";

                case "Structured":
                    return "DataTable";

                case "geography":
                    return "geography";

                default:
                    // Fallback to be manually handled by user
                    return columnType;
            };
        }
    }
}
