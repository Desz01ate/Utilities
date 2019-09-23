using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Utilities.SQL
{
    /// <summary>
    /// SQLite Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        public SQLite(string connectionString) : base(connectionString)
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
                return "TEXT";
            }
            else if (type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?))
            {
                return "INTEGER";
            }
            else if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
            {
                return "INTEGER";
            }
            else if (type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?))
            {
                return "INTEGER";
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return "INTEGER";
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return "INTEGER";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return "NUMERIC";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "INTEGER";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "NUMERIC";
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "TEXT";
            }
            else if (type == typeof(byte) || type == typeof(byte?) || type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return "INTEGER";
            }
            else if (type == typeof(byte[]))
            {
                return "BLOB";
            }
            else
            {
                throw new NotSupportedException($"Unable to map type {type.FullName} to {this.GetType().FullName} SQL Type");
            }
        }
    }
}
