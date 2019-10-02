using System.Data.SqlClient;
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
