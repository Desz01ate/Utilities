using NUnit.Framework;
using System.Data.SqlClient;
using Utilities.DesignPattern.UnitOfWork.Strategy.NonSingleton;
using Utilities.DesignPattern.UnitOfWork.Strategy.Singleton;
using Utilities.Shared;

namespace Utilities.Testing
{
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            var connectionString = "Server=localhost;Database=Local;User=sa;Password=qweQWE123;";






















            var csharpStrategy = new CSharpSingletonStrategy<SqlConnection>(connectionString, @"C:\Users\kunvu\source\repos\Playground\Playground\Singleton", "PlaygroundS");
            var visualBasicStrategy = new VisualBasicSingletonStrategy<SqlConnection>(connectionString, @"C:\Users\kunvu\source\repos\Playground\PlaygroundVB\Singleton", "TestS");
            var csharpStrategy2 = new CSharpNonSingletonStrategy<SqlConnection>(connectionString, @"C:\Users\kunvu\source\repos\Playground\Playground\NonSingleton", "PlaygroundNS");
            var visualBasicStrategy2 = new VisualBasicNonSingletonStrategy<SqlConnection>(connectionString, @"C:\Users\kunvu\source\repos\Playground\PlaygroundVB\NonSingleton", "TestNS");


            var generator = new Utilities.DesignPattern.UnitOfWork.Generator.UnitOfWorkGenerator<SqlConnection>();
            generator.UseStrategy(csharpStrategy);
            generator.Generate();
            generator.UseStrategy(csharpStrategy2);
            generator.Generate();
            generator.UseStrategy(visualBasicStrategy);
            generator.Generate();
            generator.UseStrategy(visualBasicStrategy2);
            generator.Generate();
        }
    }

}
