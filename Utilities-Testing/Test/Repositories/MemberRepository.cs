using System.Data.SqlClient;
using Utilities.Interfaces;
using Utilities.DesignPattern.UnitOfWork.Components;
using Test.Models;

namespace Test.Repositories
{
    public class MemberRepository : Repository<Member,SqlConnection,SqlParameter>
    {
       public MemberRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
