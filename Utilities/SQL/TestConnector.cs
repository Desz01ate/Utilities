using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.SQL
{
    class TestConnector : IDatabaseConnector<SqlConnection, SqlParameter>
    {
        public string ConnectionString => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        public int Delete<T>(T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync<T>(T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<SqlParameter> parameters, Func<DbDataReader, T> objectBuilder, CommandType commandType = CommandType.Text) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<SqlParameter> parameters, Func<DbDataReader, T> objectBuilder, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<SqlParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            throw new NotImplementedException();
        }

        public int Insert<T>(T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Select<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T Select<T>(object primaryKey) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> SelectAsync<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<T> SelectAsync<T>(object primaryKey) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public int Update<T>(T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync<T>(T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
