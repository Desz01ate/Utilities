using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Utilities.SQL;

namespace Utilities.Interfaces
{
    interface IDatabaseConnector<TDatabaseType, TParameter> : IDisposable, IDatabaseConnectorProperty
        where TDatabaseType : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        TDatabaseType Connection { get; }
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        T ExecuteScalar<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : struct;
        int ExecuteNonQuery(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : struct;
        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        IEnumerable<T> Select<T>() where T : class, new();
        T Select<T>(object primaryKey) where T : class, new();
        int Insert<T>(T obj) where T : class, new();
        int Update<T>(T obj) where T : class, new();
        int Delete<T>(T obj) where T : class, new();
        Task<IEnumerable<T>> SelectAsync<T>() where T : class, new();
        Task<T> SelectAsync<T>(object primaryKey) where T : class, new();
        Task<int> InsertAsync<T>(T obj) where T : class, new();
        Task<int> UpdateAsync<T>(T obj) where T : class, new();
        Task<int> DeleteAsync<T>(T obj) where T : class, new();
    }
}
