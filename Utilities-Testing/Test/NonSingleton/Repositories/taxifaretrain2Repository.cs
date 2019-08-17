using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing2.Models;

namespace Utilities.Testing2.Repositories
{
    public class taxifaretrain2Repository : Repository<taxifaretrain2,SqlConnection,SqlParameter>
    {
       public taxifaretrain2Repository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
