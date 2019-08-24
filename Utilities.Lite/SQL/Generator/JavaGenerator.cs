﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.SQL.Generator;
using Utilities.SQL.Generator.Model;

namespace Utilities.SQL.Generator
{
    public class JavaGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public JavaGenerator(string connectionString, string directory, string @namespace) : base(connectionString, directory, @namespace)
        {
        }

        protected override string GetNullableDataType(SqlColumn column)
        {
            var typejava = DataTypeMapper(column);
            return typejava;
        }
        protected override void GenerateCodeFile(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public class {table.Name}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var type = GetNullableDataType(column);
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($@"    private {type} {col};");
                sb.AppendLine($@"    public {type} get{col}() {{ return this.{col}; }}");
                sb.AppendLine($@"    public void set{col}({type} value) {{ this.{col} = value; }}");
                sb.AppendLine();
            }
            sb.AppendLine("}");
            var filePath = Path.Combine(Directory, $@"{table.Name}.class");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        protected override string DataTypeMapper(SqlColumn sqlColumn)
        {
            switch (sqlColumn.DataTypeName)
            {
                case "bit":
                    return "boolean";

                case "tinyint":
                    return "byte";
                case "smallint":
                    return "short";
                case "int":
                    return "int";
                case "bigint":
                    return "long";

                case "real":
                    return "float";
                case "decimal":
                case "money":
                case "smallmoney":
                case "float":
                    return "double";

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
                    return "String";

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                    return "byte[]";

                case "uniqueidentifier":
                    return "UDID";
                default:
                    // Fallback to be manually handled by user
                    return sqlColumn.DataTypeName;
            };
        }
    }
}
