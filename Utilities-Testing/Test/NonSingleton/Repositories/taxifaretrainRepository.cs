using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing2.Models;

namespace Utilities.Testing2.Repositories
{
    public class taxifaretrainRepository : Repository<taxifaretrain,SqlConnection,SqlParameter>
    {
       public taxifaretrainRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
