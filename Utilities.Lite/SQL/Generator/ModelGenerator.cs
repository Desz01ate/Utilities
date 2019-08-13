using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string _namespace { get; }
        /// <summary>
        /// Active table of current database.
        /// </summary>
        public List<string> Tables { get; } = new List<string>();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="outputDirectory">output directory</param>
        public ModelGenerator(string connectionString, string outputDirectory, string targetNamespace)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            _connectionString = connectionString;
            _outputDirectory = outputDirectory;
            _namespace = targetNamespace;
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
            connection.Close();
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
                case TargetLanguage.Java:
                    GenerateJavaCode(table, targetLanguage);
                    break;
                case TargetLanguage.PHP:
                    GeneratePHPCode(table);
                    break;
                case TargetLanguage.Python:
                    GeneratePythonCode(table);
                    break;
                case TargetLanguage.Python3_7:
                    GeneratePython37Code(table);
                    break;
            }
        }
        public void GenerateFromTable(string tableName, Action<Table> parser)
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
            connection.Close();
            parser(table);
        }
        private string GetNullableDataType(SqlColumn sqlColumn, TargetLanguage targetLanguage)
        {
            switch (targetLanguage)
            {
                case TargetLanguage.CSharp:
                    var typecs = DataTypeMapperCSharp(sqlColumn);
                    var addNullability = sqlColumn.AllowDBNull && typecs != "string" && typecs != "byte[]";
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
                case TargetLanguage.Java:
                    var typejava = DataTypeMapperJava(sqlColumn);
                    return typejava;
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
        private string ColumnNameCleanser(string value)
        {
            var v = new Regex("[-\\s]").Replace(value, "");
            return v;
        }
        private void GeneratePHPCode(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<?php");
            sb.AppendLine($"class {table.Name}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($"    var ${col};");
                sb.AppendLine($"    function get{col}(){{");
                sb.AppendLine($"        return $this->{col};");
                sb.AppendLine($"    }}");
                sb.AppendLine($"    function set{col}($value){{");
                sb.AppendLine($"        $this->0h{col} = $value;");
                sb.AppendLine($"    }}");
            }
            sb.AppendLine("}");
            sb.AppendLine("?>");
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.php");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private void GenerateJavaCode(Table table, TargetLanguage targetLanguage)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public class {table.Name}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var type = GetNullableDataType(column, targetLanguage);
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($@"    private {type} {col};");
                sb.AppendLine($@"    public {type} get{col}() {{ return this.{col}; }}");
                sb.AppendLine($@"    public void set{col}({type} value) {{ this.{col} = value; }}");
                sb.AppendLine();
            }
            sb.AppendLine("}");
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.class");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private void GeneratePythonCode(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"class {table.Name}:");
            sb.AppendLine($@"  def __init__(self):");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($@"    self.{col} = None");
            }
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.py");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private void GeneratePython37Code(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"import datetime");
            sb.AppendLine();
            sb.AppendLine($@"@dataclass");
            sb.AppendLine($@"class {table.Name}:");
            foreach (var column in table.Columns)
            {
                var type = DataTypeMapperPython37(column);
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($@"    {col}: {type}");
            }
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}_37.py");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private void GenerateTypeScriptCode(Table table, TargetLanguage targetLanguage)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"class {table.Name.Replace("-", "")}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($"    {col} : {GetNullableDataType(column, targetLanguage)};");
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
            if (!string.IsNullOrWhiteSpace(_namespace))
            {
                sb.AppendLine($@"Namespace {_namespace}");
            }
            sb.AppendLine($@"Public Class {table.Name.Replace("-", "")}");
            foreach (var column in table.Columns)
            {
                sb.AppendLine();
                var type = String.ToLeadingUpper(GetNullableDataType(column, targetLanguage));
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($"    Private _{col} As {type}");
                sb.AppendLine();
                sb.AppendLine($"    Public Property {col} As {type}");
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
            if (!string.IsNullOrWhiteSpace(_namespace))
            {
                sb.AppendLine($@"End Namespace");
            }
            var filePath = Path.Combine(_outputDirectory, $@"{table.Name}.vb");
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private void GenerateCSharpCode(Table table, TargetLanguage targetLanguage)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(_namespace))
            {
                sb.AppendLine($@"namespace {_namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($@"public class {table.Name.Replace("-", "")}");
            sb.AppendLine("{");
            foreach (var column in table.Columns)
            {
                var col = ColumnNameCleanser(column.ColumnName);
                sb.AppendLine($"    public {GetNullableDataType(column, targetLanguage)} {col} {{ get; set; }}");
            }
            sb.AppendLine("}");
            if (!string.IsNullOrWhiteSpace(_namespace))
            {
                sb.AppendLine("}");
            }
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
        private string DataTypeMapperPython37(SqlColumn sqlColumn)
        {
            switch (sqlColumn.DataTypeName)
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
                    return sqlColumn.DataTypeName;
            };
        }
        private string DataTypeMapperJava(SqlColumn sqlColumn)
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
