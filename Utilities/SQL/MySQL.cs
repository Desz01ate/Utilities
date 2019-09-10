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
        public MySQL(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");

        }
    }
}
