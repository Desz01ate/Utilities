using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing1.Models;

namespace Utilities.Testing1.Repositories
{
    public class UsersRepository : Repository<Users,SqlConnection,SqlParameter>
    {
       public UsersRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
