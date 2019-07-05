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
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="objectBuilder">How the POCO should build with each giving row of SqlDataReader</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        T ExecuteScalar<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        int ExecuteNonQuery(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="objectBuilder">How the POCO should build with each giving row of SqlDataReader</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : class, new();
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);
        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text);

        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable of object</returns>
        IEnumerable<T> Select<T>() where T : class, new();
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <returns>Object of given class</returns>
        T Select<T>(object primaryKey) where T : class, new();
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Affected row after an insert.</returns>
        int Insert<T>(T obj) where T : class, new();
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <returns>Affected row after an update.</returns>
        int Update<T>(T obj) where T : class, new();
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        int Delete<T>(T obj)  where T : class, new();
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable of object</returns>
        Task<IEnumerable<T>> SelectAsync<T>() where T : class, new();
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <returns>Object of given class</returns>
        Task<T> SelectAsync<T>(object primaryKey) where T : class, new();
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Affected row after an insert.</returns>
        Task<int> InsertAsync<T>(T obj);
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <returns>Affected row after an update.</returns>
        Task<int> UpdateAsync<T>(T obj)where T : class, new();
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(T obj) where T : class, new();
    }
}
