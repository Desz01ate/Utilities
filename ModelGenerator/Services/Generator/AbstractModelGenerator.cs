﻿using ModelGenerator.Services.Generator.Interfaces;
using ModelGenerator.Services.Generator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.Classes;
using Utilities.SQL.Extension;

namespace ModelGenerator.Services.Generator
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
            System.IO.Directory.CreateDirectory(directory);
            using var connection = new TDatabase()
            {
                ConnectionString = connectionString
            };
            connection.Open();
            var tables = connection.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                var database = row[0].ToString();
                var schema = row[1].ToString();
                var name = row[2].ToString();
                var type = row[3].ToString();

                Tables.Add(name);
            }
            //List<string> ls = new List<string>();
            //foreach (DataRow row in indexes.Rows)
            //{
            //    var constraint_catalog = row[0].ToString();
            //    var constraint_schema = row[1].ToString();
            //    var constraint_name = row[2].ToString();
            //    var table_catalog = row[3].ToString();
            //    var table_schema = row[4].ToString();
            //    var table_name = row[5].ToString();
            //    var index_name = row[6].ToString();
            //    var type_desc = row[7].ToString();
            //}
            tables.Dispose();
            //indexes.Dispose();
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
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException("tableName must not be null");
            if (!Tables.Contains(tableName)) throw new KeyNotFoundException("Table name not found.");
            using (var connection = new TDatabase()
            {
                ConnectionString = ConnectionString
            })
            {
                connection.Open();
                var columns = connection.GetSchemaOf(TableNameCleanser(tableName));
                string primaryKey = null;
                using (var indexes = connection.GetSchema("IndexColumns", new string[] { null, null, tableName }))
                {
                    if (indexes != null)
                    {
                        foreach (DataRow rowInfo in indexes.Rows)
                        {
                            primaryKey = rowInfo["column_name"].ToString();
                        }
                    }
                }
                var table = new Table()
                {
                    Name = tableName,
                    Columns = columns,
                    PrimaryKey = primaryKey
                };
                connection.Close();
                parser(table);
            }
        }
        protected abstract void GenerateCodeFile(Table table);
        protected virtual string GetNullableDataType(TableSchema column)
        {
            throw new NotImplementedException();
        }
        protected virtual string DataTypeMapper(TableSchema column)
        {
            throw new NotImplementedException();
        }
    }
}
