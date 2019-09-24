#if NETSTANDARD2_0
    using System.Data.SqlClient;
#elif NETSTANDARD2_1
    using Microsoft.Data.SqlClient;
#endif
namespace Utilities.SQL
{
    /// <summary>
    /// SQLServer Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class SQLServer : DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServer(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LEN");
        }
    }
}
