using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities.SQL.Abstract;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;
using Utilities.Shared;
using System.Collections.Generic;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        [Test]
        public async Task Playground()
        {
            using var con = new SQLServer("server=localhost;database=local;user=sa;password=sa;");
            var result = await con.ExecuteReaderAsyncEnumerable<taxifaretest>($@"SELECT TOP(1000) * FROM [taxi-fare-test]").ToListAsync();
            //Console.ReadLine();
        }
    }
}