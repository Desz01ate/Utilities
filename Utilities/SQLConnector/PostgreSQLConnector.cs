using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.SQLConnector
{
    class PostgreSQLConnector : DatabaseConnector<Npgsql.NpgsqlConnection, Npgsql.NpgsqlParameter>
    {
        public PostgreSQLConnector(string connectionString) : base(connectionString)
        {
        }
    }
}
