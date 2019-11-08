using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Utilities.Enum;
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

        /// <summary>
        /// Connection string of this object.
        /// </summary>
        public string ConnectionString => Connection.ConnectionString;

        /// <summary>
        /// Determine whether the connection is open or not.
        /// </summary>
        public bool IsOpen => Connection != null && Connection.State == ConnectionState.Open;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string for database.</param>
        public DatabaseConnector(string connectionString)
        {
            SQLFunctionConfiguration ??= new Dictionary<SqlFunction, string>();
            Connection = new TDatabaseConnection()
            {
                ConnectionString = connectionString
            };
            Connection.Open();
        }

        /// <summary>
        /// Protected implementation of dispose pattern
        /// </summary>
        /// <param name="disposing">.</param>
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

        /// <summary>
        /// Shortcut for this.Connection.BeginTransaction()
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// Shortcut for this.Connection.BeginTransaction(isolationLevel)
        /// </summary>
        /// <param name="isolationLevel">Specified isolation level.</param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>

        public virtual IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text) where T : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            var cursor = command.ExecuteReader();
            foreach (var result in DataReaderBuilderSync<T>(cursor))
            {
                yield return result;
            }
        }

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of dynamic object</returns>

        public virtual IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            var cursor = command.ExecuteReader();
            foreach (var result in DataReaderDynamicBuilderSync(cursor))
            {
                yield return result;
            }

        }

        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>

        public virtual T ExecuteScalar<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            T result = default;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
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

        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>

        public virtual int ExecuteNonQuery(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            int result = -1;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
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

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>

        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>

        public virtual async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text) where T : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var converter = new Converter<T>(cursor);
            var deferred = DataReaderBuilderSync<T>(cursor);
            return deferred;
        }

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <returns>IEnumerable of dynamic object</returns>

        public virtual async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            using var command = Connection.CreateCommand();

            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var deferred = DataReaderDynamicBuilderSync(cursor);
            return deferred;
        }

        /// <summary>
        /// Execute SELECT SQL query and return a scalar object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>

        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            T result = default;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                result = (T)(await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
            return result;
        }

        /// <summary>
        /// Execute any non-DML SQL Query
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>

        public virtual async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            int result = -1;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Execute SELECT SQL query and return a string
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>

        public virtual object ExecuteScalar(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            object result = null;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                result = command.ExecuteScalar();
            }
            return result;
        }

        /// <summary>
        /// Execute SELECT SQL query and return a string in asynchronous manner
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>

        public virtual async Task<object> ExecuteScalarAsync(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            object result = null;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                result = await command.ExecuteScalarAsync().ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Execute SELECT SQL query and return DataTable
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                var cursor = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(cursor);
                return dataTable;
            }
        }

        /// <summary>
        /// Execute SELECT SQL query and return DataTable in an asynchronous manner
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<TParameterType> parameters = null, IDbTransaction transaction = null, CommandType commandType = CommandType.Text)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            DbDataReader cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var dataTable = new DataTable();
            dataTable.Load(cursor);
            return dataTable;
        }
        private static IEnumerable<dynamic> DataReaderDynamicBuilderSync(DbDataReader reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return Shared.DataExtension.RowBuilder(reader);
                }
            }

        }
        private static IEnumerable<T> DataReaderBuilderSync<T>(DbDataReader reader) where T : class
        {
            using (reader)
            {
                var converter = new Converter<T>(reader);
                while (reader.Read())
                {
                    yield return converter.GenerateObject();
                }
            }
        }
    }
}