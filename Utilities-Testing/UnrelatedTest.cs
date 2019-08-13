using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Testing.SQLConnectors;

namespace Utilities.Testing
{
    [TestFixture]
    class UnrelatedTest
    {
        [Test]
        public void Playground()
        {
            using (var connection = new SQLServer("Server=localhost;Database=Local;user=sa;password=sa;"))
            {
                var taxi = new taxifaretest()
                {
                    vendor_id = "VTS"
                };
                var taxies = connection.Select<taxifaretest>(x => x.vendor_id == taxi.vendor_id);
            }
        }
    }
}
