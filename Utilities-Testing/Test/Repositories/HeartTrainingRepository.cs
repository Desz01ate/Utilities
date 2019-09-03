using System.Data.SqlClient;
using Utilities.Interfaces;
using Utilities.DesignPattern.UnitOfWork.Components;
using Test.Models;

namespace Test.Repositories
{
    public class HeartTrainingRepository : Repository<HeartTraining,SqlConnection,SqlParameter>
    {
       public HeartTrainingRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
