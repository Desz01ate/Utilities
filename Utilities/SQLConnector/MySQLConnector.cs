using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.SQLConnector
{
    class MySQLConnector : DatabaseConnector<MySqlConnection, MySqlParameter>
    {
        public MySQLConnector(string connectionString) : base(connectionString)
        {
        }
    }
}
