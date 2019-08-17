using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Utilities.Asp.Core.Repository.Boilerplate;
using Utilities.Interfaces;
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
            //var outputDir = $@"C:\Users\TYCHE\Documents\GitHub\Utilities\Utilities-Testing";
            //var targetNamespace = "Utilities.Testing";
            //BoilerplateGenerator.GenerateRepositoryService(
            //    connectionString, outputDir, targetNamespace);
            using (var con = new SQLServer(connectionString))
            {
                var data = con.Select<Models.iris>(x => !string.IsNullOrEmpty(x.Label) && x.SepalLength > 3);
                var data2 = con.Select<Models.iris>(x => !string.IsNullOrEmpty(x.Label));
            }

        }
    }
}
