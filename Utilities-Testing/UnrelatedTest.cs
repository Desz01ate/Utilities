using NUnit.Framework;
using System.Data.SqlClient;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;

namespace Utilities.Testing
{
    [TestFixture]
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            var connectionString = "Server=localhost;Database=Local;user=sa;password=sa;";
            //var outputDir = $@"C:\Users\kunvu\source\repos\Utilities\Utilities-Testing\RepositoryStorage";
            //var targetNamespace = "Utilities.Testing";
            //var generator = new Utilities.DesignPattern.Repository.SingletonServiceGenerator<SqlConnection>(connectionString, outputDir, targetNamespace);
            //generator.Generate();
            using (var con = new SQLServer(connectionString))
            {
                con.CreateTable<TestTable>();
            }
        }
    }
}
