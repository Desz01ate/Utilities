using System.Data.SqlClient;
using Utilities.Interfaces;
using Utilities.DesignPattern.UnitOfWork.Components;
using Test.Models;

namespace Test.Repositories
{
    public class irisRepository : Repository<iris, SqlConnection, SqlParameter>
    {
        public irisRepository(IDatabaseConnectorExtension<SqlConnection, SqlParameter> connector) : base(connector)
        {
        }
    }
}
