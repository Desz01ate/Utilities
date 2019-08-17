using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Utilities.DesignPattern;
using Utilities.Interfaces;
using Utilities.Testing.Models;

namespace Utilities.Testing.Repositories
{
    public class TestTableRepository : Repository<TestTable, SqlConnection, SqlParameter>
    {
        public TestTableRepository(IDatabaseConnectorExtension<SqlConnection, SqlParameter> databaseConnector) : base(databaseConnector)
        {

        }
        public void CreateTable()
        {
            _databaseConnector.CreateTable<TestTable>();
        }
        public new int Insert(TestTable obj)
        {
            var result = _databaseConnector.Insert(obj);
            return result;
        }
        public new int Update(TestTable obj)
        {
            var result = _databaseConnector.Update(obj);
            return result;
        }
        public new int Delete(TestTable obj)
        {
            var result = _databaseConnector.Delete(obj);
            return result;
        }
    }
}
