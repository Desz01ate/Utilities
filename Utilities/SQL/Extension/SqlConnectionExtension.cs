using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Utilities.Classes;
using Utilities.Shared;

namespace Utilities.SQL.Extension
{
    public static class SqlConnectionExtension
    {
        public static IEnumerable<TableSchema> GetSchemaOf<TDatabaseConnection>(this TDatabaseConnection connection, string tableName)
            where TDatabaseConnection : DbConnection, new()
        {
            if (connection == null) throw new ArgumentNullException("Connection must not be null");
            if (connection.State != System.Data.ConnectionState.Open) throw new Exception("Connection is not open.");
            var query = $"SELECT * FROM {tableName} WHERE 1 = 0";
            var command = connection.CreateCommand();
            command.CommandText = query;
            using var reader = command.ExecuteReader();
            var schema = reader.GetSchemaTable();
            var result = schema.ToList<TableSchema>();
            return result;
        }
    }
}
