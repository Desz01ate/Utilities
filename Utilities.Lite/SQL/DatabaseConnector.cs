using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enumerables;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Translator;

namespace Utilities.SQL
{
    /// <summary>
    /// Abstract class that is contains the implementation of the generic database connector.
    /// </summary>
    /// <typeparam name="TDatabaseConnection">Type of DbConnection</typeparam>
    /// <typeparam name="TParameterType">Type of DbParameter</typeparam>
    public partial class DatabaseConnector<TDatabaseConnection, TParameterType> : IDatabaseConnector<TDatabaseConnection, TParameterType>
        where TDatabaseConnection : DbConnection, new()
        where TParameterType : DbParameter, new()
    {
        /// <summary>
        /// Instance of object that hold information of the connection.
        /// </summary>
        public TDatabaseConnection Connection { get; }
        private bool Disposed { get; set; }
        public Dictionary<SqlFunction, string> SQLFunctionConfiguration { get; }
        /// <summary>
        /// Connection string of this object.
        /// </summary>
        public virtual string ConnectionString => Connection.ConnectionString;
        /// <summary>
        /// Determine wheter the connection is open or not.
        /// </summary>
        public virtual bool IsOpen => Connection != null && Connection.State == ConnectionState.Open;
        /// <summary>
        /// Determine wether the connection use transaction or not
        /// </summary>
        public bool IsPendingTransaction { get; protected set; }
        private DbTransaction Transaction { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string for database</param>
        public DatabaseConnector(string connectionString)
        {
            SQLFunctionConfiguration = new Dictionary<SqlFunction, string>();
            Connection = new TDatabaseConnection()
            {
                ConnectionString = connectionString
            };
            Connection.Open();
        }
        /// <summary>
        /// Protected implementation of dispose pattern
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                Connection.Close();
            }
            Disposed = true;
        }
        /// <summary>
        /// Object disposer which close the connection related to this object.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private DbTransaction InternalBeginTransaction()
        {
            if (IsPendingTransaction)
            {
                return Transaction;
            }
            return null;
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="objectBuilder">How the POCO should build with each giving row of SqlDataReader</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public virtual IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameterType> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where T : class, new()
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                List<T> result = new List<T>();

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    using (var cursor = command.ExecuteReader())
                    {
                        while (cursor.Read())
                        {
                            result.Add(objectBuilder(cursor));
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public virtual IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where T : class, new()
        {
            return ExecuteReader(sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        public virtual IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                List<dynamic> result = new List<dynamic>();
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    using (var cursor = command.ExecuteReader())
                    {
                        var columns = Enumerable.Range(0, cursor.FieldCount).Select(cursor.GetName).ToList();
                        while (cursor.Read())
                        {
                            result.Add(Data.RowBuilder(cursor, columns));
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public virtual T ExecuteScalar<T>(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : struct
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                T result = default;

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    result = (T)command.ExecuteScalar();
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public virtual int ExecuteNonQuery(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                int result = -1;

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    result = command.ExecuteNonQuery();
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="objectBuilder">How the POCO should build with each giving row of SqlDataReader</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public virtual async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameterType> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where T : class, new()
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                List<T> result = new List<T>();

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    using (var cursor = await command.ExecuteReaderAsync())
                    {
                        while (await cursor.ReadAsync())
                        {
                            result.Add(objectBuilder(cursor));
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public virtual async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
         where T : class, new()
        {
            return await ExecuteReaderAsync<T>(sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        public virtual async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                List<dynamic> result = new List<dynamic>();

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    using (var cursor = await command.ExecuteReaderAsync())
                    {
                        var columns = Enumerable.Range(0, cursor.FieldCount).Select(cursor.GetName).ToList();
                        while (await cursor.ReadAsync())
                        {
                            result.Add(Data.RowBuilder(cursor, columns));
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : struct
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                T result = default;

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    result = (T)(await command.ExecuteScalarAsync());
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public virtual async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameterType> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = InternalBeginTransaction();
            try
            {
                int result = -1;

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    result = await command.ExecuteNonQueryAsync();
                }

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Starts a database transaction (if you call this method again without Commit,Rollback the engine will Rollback all your changes).
        /// </summary>
        public void BeginTransaction()
        {
            if (IsPendingTransaction)
            {
                Rollback();
            }
            Transaction = Connection.BeginTransaction();
            IsPendingTransaction = true;
        }
        /// <summary>
        /// Commits the database transaction if there is open one.
        /// </summary>
        public void Commit()
        {
            if (IsPendingTransaction)
            {
                Transaction?.Commit();
                IsPendingTransaction = false;
            }
        }
        /// <summary>
        /// Rolls back a transaction from a pending state if there is open one.
        /// </summary>
        public void Rollback()
        {
            if (IsPendingTransaction)
            {
                Transaction?.Rollback();
                IsPendingTransaction = false;
            }
        }
    }
}
