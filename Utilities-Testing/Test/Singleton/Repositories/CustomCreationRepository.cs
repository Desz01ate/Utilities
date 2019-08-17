using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing1.Models;

namespace Utilities.Testing1.Repositories
{
    public class CustomCreationRepository : Repository<CustomCreation,SqlConnection,SqlParameter>
    {
       public CustomCreationRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
