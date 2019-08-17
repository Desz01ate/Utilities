using System;
using System.Data.SqlClient;
using Utilities.DesignPattern;
using Utilities.SQL;
using Utilities.Interfaces;
using Utilities.Testing1.Models;

namespace Utilities.Testing1.Repositories
{
    public class EventLogRepository : Repository<EventLog,SqlConnection,SqlParameter>
    {
       public EventLogRepository(IDatabaseConnectorExtension<SqlConnection,SqlParameter> connector) : base(connector)
       {
       }
    }
}
