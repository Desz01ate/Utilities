using ModelGenerator.Services.Generator.Interfaces;
using ModelGenerator.Services.Generator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
        public List<StoredProcedureSchema> StoredProcedures { get; private set; }
        public Func<string, string> TableNameCleanser { get; private set; } = (x) => x;

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
            tables.Dispose();


            var restrictions = new string[] { null, null, null, "PROCEDURE" };
            try
            {
                using var procedures = connection.GetSchema("Procedures", restrictions);
                StoredProcedures = connection.GetStoredProcedures().ToList();
                foreach (var sp in StoredProcedures)
                {
                    foreach (var param in sp.Parameters)
                    {
                        param.DATA_TYPE = DataTypeMapper(param.DATA_TYPE);
                    }
                }
            }
            catch
            {

            }
            //StoredProcedures = procedures.ToEnumerable(x => Data.RowBuilderStrict<StoredProcedureSchema>(x)).ToList();
            //foreach (var sp in StoredProcedures)
            //{
            //    using var spParams = connection.GetSchema("ProcedureParameters", new[] { null, null, sp.SPECIFIC_NAME, null });
            //    var param = spParams.ToEnumerable<StoredProcedureParameter>().ToArray();
            //    foreach (var p in param)
            //    {
            //        p.DATA_TYPE = DataTypeMapper(p.DATA_TYPE);
            //    }
            //    sp.Parameters = param;
            //}
        }
        public void SetCleanser(Func<string, string> func)
        {
            this.TableNameCleanser = func;
        }
        public void GenerateAllTable()
        {
            foreach (var table in Tables)
            {
                GenerateFromSpecificTable(table);
            }
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
                var columns = connection.GetTableSchema(TableNameCleanser(tableName));
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
        protected abstract string GetNullableDataType(TableSchema column);
        protected abstract string DataTypeMapper(string column);
    }
}
