using System.Data.SqlClient;
using Utilities.Interfaces;
using Utilities.DesignPattern.UnitOfWork.Components;
using Test.Models;

namespace Test.Repositories
{
    public class taxifaretrain2Repository : Repository<taxifaretrain2,SqlConnection,SqlParameter>
    {
       public taxifaretrain2Repository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
