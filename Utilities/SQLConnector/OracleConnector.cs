using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.SQLConnector
{
    class OracleConnector : DatabaseConnector<OracleConnection, OracleParameter>
    {
        public OracleConnector(string connectionString) : base(connectionString)
        {
        }
    }
}
