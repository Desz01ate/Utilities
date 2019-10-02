using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Utilities.Classes;
using Utilities.Enum;
using Utilities.Shared;

namespace Utilities.SQL.Extension
{
    public static class SqlConnectionExtension
    {
        /// <summary>
        /// Provide a safe-access to available schema restrictions on SQL Server.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="restriction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable GetSchema(this DbConnection connection, SchemaRestriction restriction, params string[] parameters)
        {
            return connection?.GetSchema(restriction.ToString(), parameters);
        }

        /// <summary>
        /// Get table schema from current database connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static IEnumerable<TableSchema> GetTableSchema(this IDbConnection connection, string tableName)
        {
            if (connection == null) throw new ArgumentNullException("Connection must not be null");
            if (connection.State != ConnectionState.Open) throw new Exception("Connection is not open.");
            var query = $"SELECT * FROM {tableName} WHERE 1 = 0";
            var command = connection.CreateCommand();
            command.CommandText = query;
            using var reader = command.ExecuteReader();
            var schema = reader.GetSchemaTable();
            var result = schema.ToEnumerable<TableSchema>();
            return result;
        }

        /// <summary>
        /// Get user-defined stored procedure from current database connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IEnumerable<StoredProcedureSchema> GetStoredProcedures(this DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("Connection must not be null");
            if (connection.State != ConnectionState.Open) throw new Exception("Connection is not open.");

            using var procedures = connection.GetSchema(SchemaRestriction.Procedures, null, null, null, "PROCEDURE");
            var storedProcedures = procedures.ToEnumerable<StoredProcedureSchema>().ToArray();
            foreach (var sp in storedProcedures)
            {
                using var spParams = connection.GetSchema(SchemaRestriction.ProcedureParameters, null, null, sp.SPECIFIC_NAME, null);
                var param = spParams.ToEnumerable<StoredProcedureParameter>();
                sp.Parameters = param.ToArray();
            }
            return storedProcedures;
        }
    }
}