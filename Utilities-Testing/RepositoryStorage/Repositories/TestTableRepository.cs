using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing.Models;

namespace Utilities.Testing.Repositories
{
    public class TestTableRepository : Repository<TestTable,SqlConnection,SqlParameter>
    {
       public TestTableRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
