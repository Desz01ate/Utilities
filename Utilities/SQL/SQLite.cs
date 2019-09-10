using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Utilities.SQL
{
    /// <summary>
    /// SQLite Connector with implementation derived from DatabaseConnector
    /// </summary>
    public sealed class SQLite : DatabaseConnector<SQLiteConnection, SQLiteParameter>
    {
        public SQLite(string connectionString) : base(connectionString)
        {
            SQLFunctionConfiguration.Add(Enum.SqlFunction.Length, "LENGTH");

        }
    }
}
