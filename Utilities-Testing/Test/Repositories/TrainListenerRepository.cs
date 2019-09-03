using System.Data.SqlClient;
using Utilities.Interfaces;
using Utilities.DesignPattern.UnitOfWork.Components;
using Test.Models;

namespace Test.Repositories
{
    public class TrainListenerRepository : Repository<TrainListener,SqlConnection,SqlParameter>
    {
       public TrainListenerRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
