using NUnit.Framework;
using System.Data.SqlClient;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;
using Utilities.Shared;
namespace Utilities.Testing
{
    class UnrelatedTest
    {
        public void Playground()
        {
            using (var connection = new SQLServer("Server=localhost;Database=Local;User=sa;Password=sa;"))
            {
                var dataset = connection.Select<taxifaretest>();
                var split = dataset.Partition(x => x.fare_amount > 150);
                var match = split.Match;
                var unmatch = split.Unmatch;
            }
        }
    }
}
