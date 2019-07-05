using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Utilities.SQL
{
    public class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        public SQLite(string connectionString) : base(connectionString)
        {

        }
    }
}
