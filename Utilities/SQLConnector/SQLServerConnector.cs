using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Utilities.SQLConnector
{
    public class SQLServerConnector : DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServerConnector(string connectionString) : base(connectionString)
        {

        }
    }
}
