using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Utilities.Enum;
using Utilities.Structs;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide methods for wrapper operation on DbConnection class
    /// </summary>
    public partial interface IDatabaseConnector : IDisposable
    {
        /// <summary>
        /// Connection string of this object.
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// Determine whether the connection is open or not.
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// Connector underlying current connection.
        /// </summary>
        DbConnection Connection { get; }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false) where T : class, new();
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <returns>IEnumerable of dynamic object</returns>
        IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false);
        /// <summary>
        /// Execute SELECT SQL query and return DataTable
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        object ExecuteScalar(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>

        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <returns>IEnumerable of POCO</returns>
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false) where T : class, new();
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <returns>IEnumerable of dynamic object</returns>
        Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false);
        /// <summary>
        /// Execute SELECT SQL query and return DataTable in an asynchronous manner
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return a string in asynchronous manner
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        string CompatibleFunctionName(SqlFunction function);
    }
}