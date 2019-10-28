using NUnit.Framework;
using System.Linq;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            var msCon = "Server=localhost;Database=Local;User=sa;Password=qweQWE123";
            var npgCon = "User Id=postgres;Password=sa;Host=localhost;Port=5432;Database=postgres";
            var affectedRow = 0;
            using (var mssqlConnector = new SQLServer(msCon))
            {
                //var d1 = mssqlConnector.Select<iris>(x => x.Label.Contains("set"));
                //var labels = new[] { "A", "B", "Iris-setosa" }.ToList();
                var labels = new[] { 1d, 2, 3, 4 };
                var data = mssqlConnector.Query<taxifaretest>(top: 1000).ToList();
                //var labels = new[] { "A", "B", "C" }.ToList();
                //var data = mssqlConnector.Select<iris>(x => labels.Contains(x.Label));
            }
        }
    }
}