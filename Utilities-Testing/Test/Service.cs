using System;
using System.Data.SqlClient;
using Utilities.SQL;
using System.Data.Common;
using Utilities.Interfaces;
using Test.Repositories;

namespace Test
{
    public sealed class Service : IUnitOfWork
    {
        private readonly static Lazy<Service> _lazyInstant = new Lazy<Service>(() => new Service());
        public readonly static Service Context = _lazyInstant.Value;
        private readonly IDatabaseConnectorExtension<SqlConnection, SqlParameter> _connection;
        Service()
        {
            _connection = new DatabaseConnector<SqlConnection, SqlParameter>("***YOUR DATABASE CREDENTIAL***");
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
        private forestfiresRepository _forestfires { get; set; }
        public forestfiresRepository forestfires
        {
            get
            {
                if (_forestfires == null)
                {
                    _forestfires = new forestfiresRepository(_connection);
                }
                return _forestfires;
            }
        }
        private MemberRepository _Member { get; set; }
        public MemberRepository Member
        {
            get
            {
                if (_Member == null)
                {
                    _Member = new MemberRepository(_connection);
                }
                return _Member;
            }
        }
        private SalesRepository _Sales { get; set; }
        public SalesRepository Sales
        {
            get
            {
                if (_Sales == null)
                {
                    _Sales = new SalesRepository(_connection);
                }
                return _Sales;
            }
        }
        private SalesCountryRepository _SalesCountry { get; set; }
        public SalesCountryRepository SalesCountry
        {
            get
            {
                if (_SalesCountry == null)
                {
                    _SalesCountry = new SalesCountryRepository(_connection);
                }
                return _SalesCountry;
            }
        }
        private TrainListenerRepository _TrainListener { get; set; }
        public TrainListenerRepository TrainListener
        {
            get
            {
                if (_TrainListener == null)
                {
                    _TrainListener = new TrainListenerRepository(_connection);
                }
                return _TrainListener;
            }
        }
        private ProjectRepository _Project { get; set; }
        public ProjectRepository Project
        {
            get
            {
                if (_Project == null)
                {
                    _Project = new ProjectRepository(_connection);
                }
                return _Project;
            }
        }
        private housingRepository _housing { get; set; }
        public housingRepository housing
        {
            get
            {
                if (_housing == null)
                {
                    _housing = new housingRepository(_connection);
                }
                return _housing;
            }
        }
        private HeartTestRepository _HeartTest { get; set; }
        public HeartTestRepository HeartTest
        {
            get
            {
                if (_HeartTest == null)
                {
                    _HeartTest = new HeartTestRepository(_connection);
                }
                return _HeartTest;
            }
        }
        private HeartTrainingRepository _HeartTraining { get; set; }
        public HeartTrainingRepository HeartTraining
        {
            get
            {
                if (_HeartTraining == null)
                {
                    _HeartTraining = new HeartTrainingRepository(_connection);
                }
                return _HeartTraining;
            }
        }
        private LineMemberRepository _LineMember { get; set; }
        public LineMemberRepository LineMember
        {
            get
            {
                if (_LineMember == null)
                {
                    _LineMember = new LineMemberRepository(_connection);
                }
                return _LineMember;
            }
        }
        private ListenerRepository _Listener { get; set; }
        public ListenerRepository Listener
        {
            get
            {
                if (_Listener == null)
                {
                    _Listener = new ListenerRepository(_connection);
                }
                return _Listener;
            }
        }
        private MembersRepository _Members { get; set; }
        public MembersRepository Members
        {
            get
            {
                if (_Members == null)
                {
                    _Members = new MembersRepository(_connection);
                }
                return _Members;
            }
        }
        private ItemRepository _Item { get; set; }
        public ItemRepository Item
        {
            get
            {
                if (_Item == null)
                {
                    _Item = new ItemRepository(_connection);
                }
                return _Item;
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
        private sysdiagramsRepository _sysdiagrams { get; set; }
        public sysdiagramsRepository sysdiagrams
        {
            get
            {
                if (_sysdiagrams == null)
                {
                    _sysdiagrams = new sysdiagramsRepository(_connection);
                }
                return _sysdiagrams;
            }
        }
        private wineRepository _wine { get; set; }
        public wineRepository wine
        {
            get
            {
                if (_wine == null)
                {
                    _wine = new wineRepository(_connection);
                }
                return _wine;
            }
        }
        private irisRepository _iris { get; set; }
        public irisRepository iris
        {
            get
            {
                if (_iris == null)
                {
                    _iris = new irisRepository(_connection);
                }
                return _iris;
            }
        }
        public DbTransaction BeginTransaction() => throw new NotImplementedException();
        public void SaveChanges(DbTransaction transaction) => throw new NotImplementedException();
        public void RollbackChanges(DbTransaction transaction) => throw new NotImplementedException();
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
