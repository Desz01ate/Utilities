using System;
using System.Linq;
using Utilities.Asp.Core.Repository;
using System.Data.SqlClient;
using Utilities.SQL;
using Utilities.Testing.Models;

namespace Utilities.Testing.Repositories
{
    public class HeartTestRepository : Repository<HeartTest>
    {
       public HeartTestRepository(DatabaseConnector<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
