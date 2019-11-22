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

            using (var connection = new SQLServer(msCon))
            {
            }
        }
    }
}