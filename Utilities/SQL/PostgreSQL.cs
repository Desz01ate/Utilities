using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Utilities.Shared;
using Utilities.SQL;

namespace Utilities.SQL
{
    /// <summary>
    /// PostgreSQL Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class PostgreSQL : DatabaseConnector<NpgsqlConnection, Npgsql.NpgsqlParameter>
    {
        public PostgreSQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enumerables.SqlFunction.Length, "LENGTH");

        }
    }
}
