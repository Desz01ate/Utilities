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
using System.Text;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        [Test]
        public async Task Playground()
        {

        }
        public string Test(IEnumerable<string> a)
        {
            foreach (var x in a)
            {
                Console.WriteLine(x);
            }
            var all = new StringBuilder();
            foreach (var x in a)
            {
                all.Append(a);
            }
            return all.ToString();
        }
    }
}