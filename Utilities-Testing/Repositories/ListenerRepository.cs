using System;
using System.Linq;
using Utilities.Asp.Core.Repository;
using System.Data.SqlClient;
using Utilities.SQL;
using Utilities.Testing.Models;

namespace Utilities.Testing.Repositories
{
    public class ListenerRepository : Repository<Listener>
    {
       public ListenerRepository(DatabaseConnector<SqlConnection,SqlParameter> connector) : base(connector)
       {

       }
    }
}
