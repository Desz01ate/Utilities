using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Utilities.SQL;

namespace Utilities.Testing.SQLConnectors
{
    class SQLServer : Utilities.SQL.DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServer(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LEN");
        }

    }
    class MySQL : DatabaseConnector<MySqlConnection, MySqlParameter>
    {
        public MySQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");
        }
    }
    class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        public SQLite(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");
        }
    }
    class PostgreSQL : DatabaseConnector<NpgsqlConnection, NpgsqlParameter>
    {
        public PostgreSQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");
        }
        public override string MapCLRTypeToSQLType(Type type)
        {
            if (type == typeof(string))
            {
                return "text";
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                return "character";
            }
            else if (type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?))
            {
                return "SMALLINT";
            }
            else if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
            {
                return "INTEGER";
            }
            else if (type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?))
            {
                return "BIGINT";
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return "REAL";
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return "FLOAT";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return "BOOLEAN";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "MONEY";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "DATE";
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "UUID";
            }
            else if (type == typeof(byte) || type == typeof(byte?) || type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return "SMALLINT";
            }
            else if (type == typeof(byte[]))
            {
                return "bytea";
            }
            else
            {
                throw new NotSupportedException($"Unable to map type {type.FullName} to {this.GetType().FullName} SQL Type");
            }
        }
    }
}
