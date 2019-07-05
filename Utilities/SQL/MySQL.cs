﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Utilities.Shared;
using Utilities.SQL;

namespace Utilities.SQL
{
    public class MySQL : DatabaseConnector<MySqlConnection, MySqlParameter>
    {
        public MySQL(string connectionString) : base(connectionString)
        {
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]
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
        public static IEnumerable<T> ExecuteReader<T>(string connectionString, string sql, IEnumerable<MySqlParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : new()
        {
            return StaticDatabseConnector.ExecuteReader<T, MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, objectBuilder, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

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
        public static IEnumerable<T> ExecuteReader<T>(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : new()
        {
            return StaticDatabseConnector.ExecuteReader<T, MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        public static IEnumerable<dynamic> ExecuteReader(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            return StaticDatabseConnector.ExecuteReader<MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

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
        public static T ExecuteScalar<T>(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            return StaticDatabseConnector.ExecuteScalar<T, MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public static int ExecuteNonQuery(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            return StaticDatabseConnector.ExecuteNonQuery<MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

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
        public static async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string connectionString, string sql, IEnumerable<MySqlParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : new()
        {
            return await StaticDatabseConnector.ExecuteReaderAsync<T, MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, objectBuilder, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

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
        public static async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : new()
        {
            return await ExecuteReaderAsync(connectionString, sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        public static async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            return await StaticDatabseConnector.ExecuteReaderAsync<MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

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
        public static async Task<T> ExecuteScalarAsync<T>(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            return await StaticDatabseConnector.ExecuteScalarAsync<T, MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public static async Task<int> ExecuteNonQueryAsync(string connectionString, string sql, IEnumerable<MySqlParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            return await StaticDatabseConnector.ExecuteNonQueryAsync<MySql.Data.MySqlClient.MySqlConnection>(connectionString, sql, parameters, commandType);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        public static IEnumerable<T> Select<T>(string connectionString)
            where T : class, new()
        {
            return StaticDatabseConnector.Select<T, MySqlConnection>(connectionString);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        public static T Select<T>(string connectionString, object primaryKey)
            where T : class, new()
        {
            return StaticDatabseConnector.Select<T, MySqlConnection, MySqlParameter>(connectionString, primaryKey);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        public static int Insert<T>(string connectionString, T obj)
            where T : class, new()
        {
            return StaticDatabseConnector.Insert<T, MySqlConnection, MySqlParameter>(connectionString, obj);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        public static int Update<T>(string connectionString, T obj)
            where T : class, new()
        {
            return StaticDatabseConnector.Update<T, MySqlConnection, MySqlParameter>(connectionString, obj);
        }
        [Obsolete("This method is deprecated and will be remove in the future, please use non-static method instead.")]

        public static int Delete<T>(string connectionString, T obj) where T : class, new()
        {
            return StaticDatabseConnector.Delete<T, MySqlConnection, MySqlParameter>(connectionString, obj);
        }
    }
}