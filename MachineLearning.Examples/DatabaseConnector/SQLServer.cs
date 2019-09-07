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
        private void SetFunctions()
        {
            this.SQLFunctionConfiguration.Add(Utilities.Enum.SqlFunction.Length, "LEN");
        }
    }
}
