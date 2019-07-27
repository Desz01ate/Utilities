﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using Utilities.SQL;

namespace Utilities.Testing.SQLConnectors
{
    class SQLServer : Utilities.SQL.DatabaseConnector<SqlConnection, SqlParameter>
    {
        public SQLServer(string connectionString) : base(connectionString)
        {
        }

    }
    class MySQL : DatabaseConnector<MySqlConnection, MySqlParameter>
    {
        public MySQL(string connectionString) : base(connectionString)
        {
        }
    }
    class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        public SQLite(string connectionString) : base(connectionString)
        {
        }
    }
}
