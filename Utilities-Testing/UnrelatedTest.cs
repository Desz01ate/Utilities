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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        public partial class Calendar
        {
            public int id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Location { get; set; }
        }
        [Test]
        public async Task Playground()
        {
            using var con = new SQLServer("server=localhost;database=local;user=sa;password=sa;");
            var start = DateTime.Now.AddDays(-10);
            var end = DateTime.Now;
            var query = Utilities.SQL.Extension.SqlQueryExtension.SelectQueryGenerate<Calendar, SqlParameter>(con, (x => DateTime.Now.AddDays(-10) <= x.StartDate && x.EndDate <= end));
            //Console.ReadLine();
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