using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.SQL.Generator.Enumerable;
using Utilities.SQL.Generator.Model;

namespace Utilities.SQL.Generator
{
    /// <summary>
    /// This class contain definition for database model generator (This class is consider a beta feature and only tested on SQLServer).
    /// </summary>
    /// <typeparam name="TDatabase"></typeparam>
    public class ModelGenerator<TDatabase> where TDatabase : DbConnection, new()
    {
        private string _connectionString { get; }
        private string _outputDirectory { get; }
        /// <summary>
        /// Active table of current database.
        /// </summary>
        public List<string> Tables { get; } = new List<string>();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="outputDirectory">output directory</param>
        public ModelGenerator(string connectionString, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            _connectionString = connectionString;
            _outputDirectory = outputDirectory;
            LoadTables();
        }
        private void LoadTables()
        {
            using var connection = new TDatabase()
            {
                ConnectionString = _connectionString
            };
            connection.Open();
            var dataTable = connection.GetSchema("Tables");
            foreach (DataRow row in dataTable.Rows)
            {
                var database = row[0].ToString();
                var schema = row[1].ToString();
                var name = row[2].ToString();
                var type = row[3].ToString();
                Tables.Add(name);
            }
        }
        /// <summary>
        /// Generate model for all tables of current database.
        /// </summary>
        /// <param name="targetLanguage">Target language of model</param>
        public void GenerateAllTables(TargetLanguage targetLanguage)
        {
            foreach (var table in Tables)
            {
                GenerateFromTable(table, targetLanguage);
            }
        }
        /// <summary>
        /// Generate model for specific table.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="targetLanguage">Target language of model</param>
        public void GenerateFromTable(string tableName, TargetLanguage targetLanguage)
        {
            if (!Tables.Contains(tableName)) throw new KeyNotFoundException("Table name not found.");
            using var connection = new TDatabase()
            {
                ConnectionString = _connectionString
            };
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT * FROM {TableNameCleanser(tableName)} WHERE 1 = 0";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 60;
            using var cursor = command.ExecuteReader();

            var columns = new List<SqlColumn>();
            var typeProperties = new[] { "DataType", "ProviderSpecificDataType" };
            var schema = cursor.GetSchemaTable();
            for (var rowIndex = 0; rowIndex < schema.Rows.Count; rowIndex++)
            {
                var sqlColumn = new SqlColumn();
                for (var colIndex = 0; colIndex < schema.Columns.Count; colIndex++)
                {
                    var propertyName = schema.Columns[colIndex].ColumnName;
                    var value = schema.Rows[rowIndex][colIndex];
                    if (typeProperties.Contains(propertyName))
                        value = ((Type)value).FullName;
                    var property = typeof(SqlColumn).GetProperty(propertyName);
                    if (!Convert.IsDBNull(value) && property != null)
                        property.SetValue(sqlColumn, value, null);
                }
                var decimalTypes = new[] { "real", "float", "decimal", "money", "smallmoney" };
                if (decimalTypes.Contains(sqlColumn.DataTypeName))
                {
                    sqlColumn.NumericScale = 0;
                    sqlColumn.NumericPrecision = 0;
                }
                columns.Add(sqlColumn);
            }

            var table = new Table()
            {
                Name = tableName,
                Columns = columns
            };
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    GenerateCSharpCode(table, targetLanguage);
                    break;
                case TargetLanguage.VisualBasic:
                    GenerateVisualBasicCode(table, targetLanguage);
                    break;
                case TargetLanguage.TypeScript:
                    GenerateTypeScriptCode(table, targetLanguage);
                    break;
            }
        }
        private string GetNullableDataType(SqlColumn sqlColumn, TargetLanguage targetLanguage)
        {
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    var typecs = DataTypeMapperCSharp(sqlColumn);
                    var addNullability = sqlColumn.AllowDBNull && typecs != "string";
                    return addNullability ? typecs + "?" : typecs;
                case TargetLanguage.VisualBasic:
                    var typevb = DataTypeMapperVisualBasic(sqlColumn);
                    if (sqlColumn.AllowDBNull && typevb != "String")
                    {
                        return $"Nullable(Of {typevb})";
                    }
                    return typevb;
                case TargetLanguage.TypeScript:
                    var typets = DataTypeMapperTypeScript(sqlColumn);
                    if (sqlColumn.AllowDBNull)
                    {
                        return $"{typets} | null";
                    }
                    return typets;
            }
            return string.Empty;
        }

        private string TableNameCleanser(string tableName)
        {
            if (tableName.Contains("-"))
            {
                return $"[{tableName}]";
            }
            return tableName;
        }

        private void GenerateTypeScriptCode(Table table, TargetLanguage targetLanguage)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"class {table.Name.Replace("-", "")}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                sb.AppendLine($"    {column.ColumnName} : {GetNullableDataType(column, targetLanguage)};");
            }
            sb.AppendLine("}");
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.ts");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        private void GenerateVisualBasicCode(Table table, TargetLanguage targetLanguage)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Imports System");
            sb.AppendLine();
            sb.AppendLine($@"Public Class {table.Name.Replace("-", "")}");
            foreach (var column in table.Columns)
            {
                sb.AppendLine();
                var type = Utilities.String.ToLeadingUpper(GetNullableDataType(column, targetLanguage));
                sb.AppendLine($"    Private _{column.ColumnName} As {type}");
                sb.AppendLine();
                sb.AppendLine($"    Public Property {column.ColumnName} As {type}");
                sb.AppendLine($"      Get");
                sb.AppendLine($"         Return _{column.ColumnName}");
                sb.AppendLine($"      End Get");
                sb.AppendLine($"      Set(value As {type})");
                sb.AppendLine($"         _{column.ColumnName} = value");
                sb.AppendLine($"      End Set");
                sb.AppendLine($"    End Property");
            }
            sb.AppendLine();
            sb.AppendLine("End Class");
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.vb");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private void GenerateCSharpCode(Table table, TargetLanguage targetLanguage)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($@"public class {table.Name.Replace("-", "")}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                sb.AppendLine($"    public {GetNullableDataType(column, targetLanguage)} {column.ColumnName} {{ get; set; }}");
            }
            sb.AppendLine("}");
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.cs");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private string DataTypeMapperCSharp(SqlColumn sqlColumn)
        {
            switch (sqlColumn.DataTypeName)
            {
                case "bit":
                    return "bool";

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
                case "float":
                    return "double";
                case "decimal":
                case "money":
                case "smallmoney":
                    return "decimal";

                case "time":
                    return "TimeSpan";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "DateTime";
                case "datetimeoffset":
                    return "DateTimeOffset";

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                    return "string";

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                    return "byte[]";

                case "uniqueidentifier":
                    return "Guid";

                case "variant":
                case "Udt":
                    return "object";

                case "Structured":
                    return "DataTable";

                case "geography":
                    return "geography";

                default:
                    // Fallback to be manually handled by user
                    return sqlColumn.DataTypeName;
            };
        }
        private string DataTypeMapperTypeScript(SqlColumn sqlColumn)
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
        private string DataTypeMapperVisualBasic(SqlColumn sqlColumn)
        {
            switch (sqlColumn.DataTypeName)
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
                    return sqlColumn.DataTypeName;
            };
        }
    }
}
