using NUnit.Framework;
using System;
using System.Text;
using Utilities.Testing.Models;
using Utilities.Shared;
using System.Linq;
using System.Data;
using Utilities.Interfaces;
using Utilities.Testing.SQLConnectors;

namespace Utilities.Testing
{
    class UnrelatedTest
    {

        [Test]
        public void Playground()
        {
            var msCon = "Server=localhost;Database=Local;User=sa;Password=sa";
            var npgCon = "User Id=postgres;Password=sa;Host=localhost;Port=5432;Database=postgres";






            var affectedRow = 0;
            using (var mssqlConnector = new SQLServer(msCon))
            {
                var tx = mssqlConnector.ExecuteReader<iris>($"SELECT * FROM [iris]");
                using (var postgresConnector = new PostgreSQL(npgCon))
                {
                    postgresConnector.DROP_TABLE_USE_WITH_CAUTION<iris>();
                    postgresConnector.CreateTable<iris>();
                    affectedRow = postgresConnector.Insert(tx);
                    postgresConnector.DROP_TABLE_USE_WITH_CAUTION<iris>();
                }
            }
        }
    }
}
