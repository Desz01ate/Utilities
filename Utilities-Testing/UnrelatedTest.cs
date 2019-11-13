using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities.SQL.Abstract;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;
using Utilities.Shared;
namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        [Test]
        public async Task Playground()
        {
            var msCon = "Server=localhost;Database=Local;User=sa;Password=sa";

            using var mssqlConnector = new SQLServer(msCon);
            var sql = Utilities.SQL.Extension.SqlQueryExtension.SelectQueryGenerate<taxifaretest, SqlParameter>(top: 10);
            foreach (var data in await mssqlConnector.ExecuteReaderAsync<taxifaretest>(sql))
            {
                
            }
            //var d1 = mssqlConnector.Select<iris>(x => x.Label.Contains("set"));
            //var labels = new[] { "A", "B", "Iris-setosa" }.ToList();
            var labels = new[] { 1d, 2, 3, 4 };
            //var data = mssqlConnector.Query<taxifaretest>(top: 1000).ToList();
            foreach (var data in await mssqlConnector.QueryAsync<taxifaretest>(top: 10))
            {
                Console.WriteLine(data.fareamount);
            }
        }
    }
}