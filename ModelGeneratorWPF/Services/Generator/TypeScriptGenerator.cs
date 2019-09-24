using ModelGenerator.Services.Generator.Model;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator
{
    public class TypeScriptGenerator<TDatabase> : AbstractModelGenerator<TDatabase>
        where TDatabase : DbConnection, new()
    {
        public TypeScriptGenerator(string connectionString, string directory, string @namespace, Func<string, string> func = null) : base(connectionString, directory, @namespace)
        {
            if (func != null) this.SetCleanser(func);
        }

        protected override string GetNullableDataType(TableSchema column)
        {
            var typets = DataTypeMapper(column.DataTypeName);
            if (column.AllowDBNull)
            {
                return $"{typets} | undefined | null";
            }
            return typets;
        }
        protected override void GenerateCodeFile(Table table)
        {
            var classesDir = Path.Combine(Directory, "TS_Classes");
            System.IO.Directory.CreateDirectory(classesDir);
            var interfaceDir = Path.Combine(Directory, "TS_Interfaces");
            System.IO.Directory.CreateDirectory(interfaceDir);

            var classSb = new StringBuilder();
            var interfaceSb = new StringBuilder();
            var propertyCode = new StringBuilder();
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                propertyCode.AppendLine($"    {col} : {GetNullableDataType(column)};");
            }
            var ppcString = propertyCode.ToString();


            classSb.AppendLine($@"class {table.Name.Replace("-", "")}");
            classSb.AppendLine("{");
            classSb.Append(ppcString);
            classSb.AppendLine("}");
            classSb.AppendLine();
            var filePath = Path.Combine(classesDir, $@"{table.Name}.ts");
            System.IO.File.WriteAllText(filePath, classSb.ToString());

            interfaceSb.AppendLine($@"interface I{table.Name.Replace("-", "")}");
            interfaceSb.AppendLine("{");
            interfaceSb.Append(ppcString);
            interfaceSb.AppendLine("}");
            var filePathInterface = Path.Combine(interfaceDir, $@"I{table.Name}.ts");
            System.IO.File.WriteAllText(filePathInterface, interfaceSb.ToString());
        }
        protected override string DataTypeMapper(string columnType)
        {
            switch (columnType)
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
