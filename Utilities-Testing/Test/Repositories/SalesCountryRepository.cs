using System.Data.SqlClient;
using Utilities.Interfaces;
using Utilities.DesignPattern.UnitOfWork.Components;
using Test.Models;

namespace Test.Repositories
{
    public class SalesCountryRepository : Repository<SalesCountry,SqlConnection,SqlParameter>
    {
       public SalesCountryRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
