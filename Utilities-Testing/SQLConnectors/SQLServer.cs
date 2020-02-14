using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using Utilities.Enum;
using Utilities.SQL;

namespace Utilities.Testing.SQLConnectors
{
    internal class SQLServer : DatabaseConnector
    {
        public SQLServer(string connectionString) : base(typeof(SqlConnection), connectionString)
        {

        }
        protected override string CompatibleSQLType(Type type)
        {
            //throw new Exception("Test thrower");
            return base.CompatibleSQLType(type);
        }
    }

    internal class MySQL : DatabaseConnector
    {
        public MySQL(string connectionString) : base(typeof(MySqlConnection), connectionString)
        {

        }
        protected override string CompatibleFunctionName(SqlFunction function)
        {
            if (function == SqlFunction.Length) return "LENGTH";
            return base.CompatibleFunctionName(function);
        }
    }

    internal class SQLite : DatabaseConnector
    {
        public SQLite(string connectionString) : base(typeof(SQLiteConnection), connectionString)
        {

        }
        protected override string CompatibleFunctionName(SqlFunction function)
        {
            if (function == SqlFunction.Length) return "LENGTH";
            return base.CompatibleFunctionName(function);
        }
    }

    internal class PostgreSQL : DatabaseConnector
    {
        public PostgreSQL(string connectionString) : base(typeof(Npgsql.NpgsqlConnection), connectionString)
        {

        }
        protected override string CompatibleFunctionName(SqlFunction function)
        {
            if (function == SqlFunction.Length) return "LENGTH";
            return base.CompatibleFunctionName(function);
        }
    }
}