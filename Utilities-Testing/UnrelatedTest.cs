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
                var d1 = mssqlConnector.Select<iris>(x => x.Label.Contains("set"));
                var labels = new[] { "A", "B", "Iris-setosa" };
                var data = mssqlConnector.Select<iris>(x => labels.Contains(x.Label));
            }
        }
    }
}
