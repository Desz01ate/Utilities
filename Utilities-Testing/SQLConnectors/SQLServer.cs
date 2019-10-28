using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using Utilities.SQL;

namespace Utilities.Testing.SQLConnectors
{
    internal class SQLServer : Utilities.SQL.DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServer(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LEN");
        }
        protected override string MapCLRTypeToSQLType(Type type)
        {
            //throw new Exception("Test thrower");
            return base.MapCLRTypeToSQLType(type);
        }
    }

    internal class MySQL : DatabaseConnector<MySqlConnection, MySqlParameter>
    {
        public MySQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");
        }
    }

    internal class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        public SQLite(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");
        }
    }

    internal class PostgreSQL : DatabaseConnector<NpgsqlConnection, NpgsqlParameter>
    {
        public PostgreSQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");
        }
    }
}