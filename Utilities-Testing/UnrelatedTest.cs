using NUnit.Framework;
using System.Data.SqlClient;
using Utilities.DesignPattern.UnitOfWork.Strategy.Singleton;
using Utilities.Shared;

namespace Utilities.Testing
{
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {

            var generator = new Utilities.DesignPattern.UnitOfWork.Generator.UnitOfWorkGenerator<SqlConnection>();
            generator.SetStrategy(new CSharpStrategy<SqlConnection>("Server=localhost;Database=Local;User=sa;Password=sa;", @"C:\Users\TYCHE\Documents\Golang", "Test"));
            generator.Generate();
        }
    }

}
