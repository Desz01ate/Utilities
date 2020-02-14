using System.Data.SqlClient;
using Utilities.SQL;

namespace MachineLearning.Examples.DataConnector
{
    class SQLServer : DatabaseConnector
    {
        public SQLServer(string connectionString) : base(typeof(System.Data.SqlClient.SqlConnection), connectionString)
        {
        }
    }
}
