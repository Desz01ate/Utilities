using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Utilities.Shared;
using Utilities.SQL;

namespace Utilities.SQL
{
    /// <summary>
    /// SQLServer Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class SQLServer : DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServer(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enumerables.SqlFunction.Length, "LEN");
        }
        public SQLServer(string connectionString, bool useTransaction) : base(connectionString, useTransaction)
        {
            SQLFunctionConfiguration.Add(Enumerables.SqlFunction.Length, "LEN");
        }
    }
}
