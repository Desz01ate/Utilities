using NUnit.Framework;
using System.Data.SqlClient;

namespace Utilities.Testing
{
    [TestFixture]
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            var connectionString = "Server=localhost;Database=Local;user=sa;password=sa;";
            var outputDir = $@"C:\Users\kunvu\source\repos\Utilities\Utilities-Testing\RepositoryStorage";
            var targetNamespace = "Utilities.Testing";
            var generator = new Utilities.DesignPattern.Repository.SingletonServiceGenerator<SqlConnection>(connectionString, outputDir, targetNamespace);
            generator.Generate();
        }
    }
}
