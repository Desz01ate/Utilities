using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Utilities.SQL;

namespace MachineLearning.Examples.DataConnector
{
    class SQLServer : DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServer(string connectionString) : base(connectionString)
        {
            SetFunctions();
        }
        public SQLServer(string connectionString, bool useTransaction) : base(connectionString, useTransaction)
        {
            SetFunctions();
        }
        private void SetFunctions()
        {
            this.SQLFunctionConfiguration.Add(Utilities.Enumerables.SqlFunction.Length, "LEN");
        }
    }
}
