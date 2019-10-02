#if !NET45

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Extension;

namespace Utilities.SQL
{
    /// <summary>
    /// Connector which internally combine the original DatabaseConnector and override some methods to take advantage of Dapper for high performance scenario.
    /// </summary>
    /// <typeparam name="TDatabaseConnection"></typeparam>
    /// <typeparam name="TParameterType"></typeparam>
    public class DapperConnector<TDatabaseConnection, TParameterType> : DatabaseConnector<TDatabaseConnection, TParameterType>
         where TDatabaseConnection : DbConnection, new()
         where TParameterType : DbParameter, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public DapperConnector(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return this.Connection.Execute(sql, parameters.ToDapperParameters(), commandType: commandType, transaction: transaction);
        }

        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return await this.Connection.ExecuteAsync(sql, parameters.ToDapperParameters(), commandType: commandType, transaction: transaction).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of dynamic.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return this.Connection.Query(sql, parameters.ToDapperParameters(), transaction, commandType: commandType);
        }

        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of T.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return this.Connection.Query<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType);
        }

        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of dynamic.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return await this.Connection.QueryAsync(sql, parameters.ToDapperParameters(), transaction, commandType: commandType).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of T.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return await this.Connection.QueryAsync<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override T ExecuteScalar<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return this.Connection.ExecuteScalar<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            return await this.Connection.ExecuteScalarAsync<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType).ConfigureAwait(false);
        }
    }
}

#endif