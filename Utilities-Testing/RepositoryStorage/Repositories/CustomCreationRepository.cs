using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing.Models;

namespace Utilities.Testing.Repositories
{
    public class CustomCreationRepository : Repository<CustomCreation,SqlConnection,SqlParameter>
    {
       public CustomCreationRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
