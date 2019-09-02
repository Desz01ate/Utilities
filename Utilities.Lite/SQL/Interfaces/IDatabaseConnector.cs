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
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, CommandType commandType = CommandType.Text, DbTransaction transaction = null) where T : class, new();
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null) where T : class, new();
        IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);

        DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);

        string ExecuteScalar(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);
        T ExecuteScalar<T>(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null) where T : struct;
        int ExecuteNonQuery(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, CommandType commandType = CommandType.Text, DbTransaction transaction = null) where T : class, new();
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null) where T : class, new();
        Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);

        Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);


        Task<string> ExecuteScalarAsync(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);

        Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null) where T : struct;
        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null);
    }
}
