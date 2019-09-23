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
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");

        }
        public override string MapCLRTypeToSQLType(Type type)
        {
            if (type == typeof(string))
            {
                return "TEXT";
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                return "CHARACTER(1)";
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
                return "DOUBLE PRECISION";
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
                return "DATETIME";
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
                return "BYTEA";
            }
            else
            {
                throw new NotSupportedException($"Unable to map type {type.FullName} to {this.GetType().FullName} SQL Type");
            }
        }
    }
}
