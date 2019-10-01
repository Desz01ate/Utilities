using NUnit.Framework;
using System;
using System.Text;
using Utilities.Testing.Models;
using Utilities.Shared;
using System.Linq;
using System.Data;
using Utilities.Interfaces;
using Utilities.Testing.SQLConnectors;
using System.Data.SqlClient;
using Utilities.SQL;
using Utilities.SQL.Extension;
namespace Utilities.Testing
{
    class UnrelatedTest
    {

        [Test]
        public void Playground()
        {
            var msCon = "Server=localhost;Database=Local;User=sa;Password=qweQWE123";
            using (var con = new DatabaseConnector<SqlConnection, SqlParameter>(msCon))
            {
                var iris = new iris()
                {
                    Label = "AAA",
                    PetalLength = 1,
                    PetalWidth = 1,
                    SepalLength = 1,
                    SepalWidth = 1
                };
                var test = con.Update(iris, x => x.Label == "AAA" && (1 < x.PetalLength || x.PetalLength < 10));
                var data = con.Select<iris>(top: 10);
            }

        }
    }
}
