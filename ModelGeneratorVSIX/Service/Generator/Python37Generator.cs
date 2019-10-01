using ModelGenerator.Services.Generator.Model;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator
{
    public class Python37Generator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public Python37Generator(string connectionString, string directory, string @namespace, Func<string, string> func = null) : base(connectionString, directory, @namespace)
        {
            if (func != null) this.SetCleanser(func);
        }

        protected override void GenerateCodeFile(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"import datetime");
            sb.AppendLine();
            sb.AppendLine($@"@dataclass");
            sb.AppendLine($@"class {table.Name}:");
            foreach (var column in table.Columns)
            {
                var type = DataTypeMapper(column.DataTypeName);
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($@"    {col}: {type}");
            }
            var filePath = Path.Combine(Directory, $@"{table.Name}_37.py");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        protected override string DataTypeMapper(string columnType)
        {
            switch (columnType)
            {
                case "bit":
                    return "bool";

                case "tinyint":
                case "smallint":
                case "int":
                case "bigint":
                    return "int";

                case "real":
                case "float":
                case "decimal":
                case "money":
                case "smallmoney":
                    return "float";

                case "time":
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "datetimeoffset":
                    return "datetime.datetime";

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                case "uniqueidentifier":
                    return "str";

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                    return "bytes";
                default:
                    // Fallback to be manually handled by user
                    return columnType;
            };
        }

        protected override string GetNullableDataType(TableSchema column)
        {
            throw new System.NotImplementedException();
        }
    }
}
