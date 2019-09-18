using Oracle.ManagedDataAccess.Client;
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
    /// Oracle Database Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class Oracle : DatabaseConnector<OracleConnection, OracleParameter>
    {
        public Oracle(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");

        }
    }
}
