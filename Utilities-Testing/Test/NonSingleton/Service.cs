using System;
using System.Data.SqlClient;
using Utilities.SQL;
using System.Data.Common;
using Utilities.Interfaces;
using Utilities.Testing2.Repositories;

namespace Utilities.Testing2
{
    public class Service : IUnitOfWork
    {
        private bool IsWaitingTransaction { get; set; }
        private DbTransaction transaction { get; set; }
        private readonly IDatabaseConnectorExtension<SqlConnection, SqlParameter> _connection;
        public Service(IDatabaseConnectorExtension<SqlConnection, SqlParameter> connector)
        {
            _connection = connector;
        }
        public Service()
        {
            _connection = new DatabaseConnector<SqlConnection, SqlParameter>("Server=localhost;Database=Local;user=sa;password=sa;");
        }
        private taxifaretestRepository _taxifaretest { get; set; }
        public taxifaretestRepository taxifaretest
        {
            get
            {
                if (_taxifaretest == null)
                {
                    _taxifaretest = new taxifaretestRepository(_connection);
                }
                return _taxifaretest;
            }
        }
        private taxifaretrainRepository _taxifaretrain { get; set; }
        public taxifaretrainRepository taxifaretrain
        {
            get
            {
                if (_taxifaretrain == null)
                {
                    _taxifaretrain = new taxifaretrainRepository(_connection);
                }
                return _taxifaretrain;
            }
        }
        private taxifaretrain2Repository _taxifaretrain2 { get; set; }
        public taxifaretrain2Repository taxifaretrain2
        {
            get
            {
                if (_taxifaretrain2 == null)
                {
                    _taxifaretrain2 = new taxifaretrain2Repository(_connection);
                }
                return _taxifaretrain2;
            }
        }
        private land_dataRepository _land_data { get; set; }
        public land_dataRepository land_data
        {
            get
            {
                if (_land_data == null)
                {
                    _land_data = new land_dataRepository(_connection);
                }
                return _land_data;
            }
        }
        private CustomCreationRepository _CustomCreation { get; set; }
        public CustomCreationRepository CustomCreation
        {
            get
            {
                if (_CustomCreation == null)
                {
                    _CustomCreation = new CustomCreationRepository(_connection);
                }
                return _CustomCreation;
            }
        }
        private coordinateRepository _coordinate { get; set; }
        public coordinateRepository coordinate
        {
            get
            {
                if (_coordinate == null)
                {
                    _coordinate = new coordinateRepository(_connection);
                }
                return _coordinate;
            }
        }
        private EventLogRepository _EventLog { get; set; }
        public EventLogRepository EventLog
        {
            get
            {
                if (_EventLog == null)
                {
                    _EventLog = new EventLogRepository(_connection);
                }
                return _EventLog;
            }
        }
        private UsersRepository _Users { get; set; }
        public UsersRepository Users
        {
            get
            {
                if (_Users == null)
                {
                    _Users = new UsersRepository(_connection);
                }
                return _Users;
            }
        }
        public void BeginTransaction()
        {


        }
        public void SaveChanges()
        {

        }
        public void RollbackChanges()
        {

        }
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
