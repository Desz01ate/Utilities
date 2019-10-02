using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide methods for wrapper operation on DbConnection class
    /// </summary>
    /// <typeparam name="TDatabaseType">DbConnection type</typeparam>
    /// <typeparam name="TParameter">DbParameter type</typeparam>
    public interface IDatabaseConnector<TDatabaseType, TParameter> : IDisposable, IDatabaseConnectorProperty
        where TDatabaseType : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        TDatabaseType Connection { get; }

        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text) where T : class, new();

        IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        string ExecuteScalar(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        T ExecuteScalar<T>(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text) where T : struct;

        int ExecuteNonQuery(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text) where T : class, new();

        Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        Task<string> ExecuteScalarAsync(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);

        Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text) where T : struct;

        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameter> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text);
    }
}