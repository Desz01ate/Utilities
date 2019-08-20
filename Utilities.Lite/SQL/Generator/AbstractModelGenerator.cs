using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.Interfaces;
using Utilities.SQL.Generator.Model;

namespace Utilities.SQL.Generator
{
    public abstract class AbstractModelGenerator<TDatabase> : IModelGenerator
        where TDatabase : DbConnection, new()
    {
        public string ConnectionString { get; protected set; }

        public string Directory { get; protected set; }

        public string Namespace { get; protected set; }

        public List<string> Tables { get; } = new List<string>();
        public AbstractModelGenerator(string connectionString, string directory, string @namespace)
        {
            ConnectionString = connectionString;
            Directory = directory;
            Namespace = @namespace;
            using var connection = new TDatabase()
            {
                ConnectionString = connectionString
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
        public void GenerateAllTable()
        {
            foreach (var table in Tables)
            {
                GenerateFromSpecificTable(table);
            }
        }
        private string TableNameCleanser(string tableName)
        {
            if (tableName.Contains("-"))
            {
                return $"[{tableName}]";
            }
            return tableName;
        }
        protected string ColumnNameCleanser(string value)
        {
            var v = new Regex("[-\\s]").Replace(value, "");
            return v;
        }
        protected void GenerateFromSpecificTable(string tableName)
        {
            GenerateFromSpecificTable(tableName, GenerateCodeFile);
        }

        public void GenerateFromSpecificTable(string tableName, Action<Table> parser)
        {
            if (!Tables.Contains(tableName)) throw new KeyNotFoundException("Table name not found.");
            using var connection = new TDatabase()
            {
                ConnectionString = ConnectionString
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
        protected virtual void GenerateCodeFile(Table table)
        {
            throw new NotImplementedException();
        }
        protected virtual string GetNullableDataType(SqlColumn column)
        {
            throw new NotImplementedException();
        }
        protected virtual string DataTypeMapper(SqlColumn column)
        {
            throw new NotImplementedException();
        }
    }
}
