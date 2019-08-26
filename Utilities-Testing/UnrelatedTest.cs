using NUnit.Framework;
using System.Data.SqlClient;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;
using Utilities.Shared;
using System.Text;

namespace Utilities.Testing
{
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            using (var con = new SQLServer("server=localhost;database=Local;user=sa;password=sa"))
            {
                var iris = con.Select<iris>(top: 10);
                Utilities.File.WriteAsCsv(iris, @"C:\Users\TYCHE\Desktop\iris.csv", Encoding.UTF8);
                Utilities.File.WriteAsJson(iris, @"C:\Users\TYCHE\Desktop\iris.json", Encoding.UTF8);
            }
        }
    }
}
