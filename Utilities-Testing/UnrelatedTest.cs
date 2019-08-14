﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Asp.Core.Repository.Boilerplate;
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
            var outputDir = $@"C:\Users\kunvu\source\repos\Utilities\Utilities-Testing";
            var targetNamespace = "Utilities.Testing";
            BoilerplateGenerator.GenerateRepositoryService(
                connectionString, outputDir, targetNamespace);
        }
    }
}