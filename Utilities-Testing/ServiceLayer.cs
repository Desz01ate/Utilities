using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Testing.SQLConnectors;
using Utilities.Asp.Core.Repository;
using System.Data.SqlClient;

namespace Utilities.Testing
{
    public class ServiceLayer
    {
        public static ServiceLayer Context { get; } = new ServiceLayer("");
        private readonly SQLServer connector;
        public Repository<taxifaretest> TaxiFareTestRepository { get; }
        public Repository<EventLog> EventLogRepository { get; }
        public Repository<coordinate> CoordinateRepository { get; }
        ServiceLayer(string connectionString)
        {
            connector = new SQLServer(connectionString);
            TaxiFareTestRepository = new Repository<taxifaretest>(connector);
            EventLogRepository = new Repository<EventLog>(connector);
            CoordinateRepository = new Repository<coordinate>(connector);
        }
        public void CloseConnection()
        {
            connector.Dispose();
        }
    }
}
