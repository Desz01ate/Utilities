using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing1.Models;

namespace Utilities.Testing1.Repositories
{
    public class land_dataRepository : Repository<land_data,SqlConnection,SqlParameter>
    {
       public land_dataRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
