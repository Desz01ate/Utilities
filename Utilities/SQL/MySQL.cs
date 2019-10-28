using MySql.Data.MySqlClient;
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
    /// MySQL Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class MySQL : DatabaseConnector<MySqlConnection, MySqlParameter>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public MySQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");

        }
        protected internal override string MapCLRTypeToSQLType(Type type)
        {
            if (type == typeof(string))
            {
                return "VARCHAR(1024)";
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                return "CHAR(1)";
            }
            else if (type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?))
            {
                return "SMALLINT";
            }
            else if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
            {
                return "INT";
            }
            else if (type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?))
            {
                return "BIGINT";
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return "FLOAT";
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return "DOUBLE";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return "BOOLEAN";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "DECIMAL(13,4)";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "DATETIME";
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "CHAR(38)";
            }
            else if (type == typeof(byte) || type == typeof(byte?) || type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return "SMALLINT";
            }
            else if (type == typeof(byte[]))
            {
                return "VARBINARY";
            }
            else
            {
                throw new NotSupportedException($"Unable to map type {type.FullName} to {this.GetType().FullName} SQL Type");
            }
        }
    }
}
