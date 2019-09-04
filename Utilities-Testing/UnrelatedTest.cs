using NUnit.Framework;
using System.Data.SqlClient;
using Utilities.Testing.SQLConnectors;

namespace Utilities.Testing
{
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            var constr = "Server=localhost;Database=Local;User=sa;Password=sa;";
            var path = @"C:\Users\TYCHE\Documents\GitHub\Utilities\Utilities-Testing\Test";
            var @namespace = "Utilities.Testing";

            using (var connector = new SQLServer(constr))
            {
            }


        }
    }

}
