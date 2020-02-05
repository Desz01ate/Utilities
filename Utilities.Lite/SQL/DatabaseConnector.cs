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
using Utilities.SQL.Translator;
using Utilities.Structs;

namespace Utilities.SQL
{
    /// <summary>
    /// Abstract class that is contains the implementation of the database connector.
    /// </summary>
    public partial class DatabaseConnector : IDatabaseConnector
    {
        /// <summary>
        /// Connection string of this object.
        /// </summary>
        public string ConnectionString => Connection.ConnectionString;
        /// <summary>
        /// Determine whether the connection is open or not.
        /// </summary>
        public bool IsOpen => Connection.State == ConnectionState.Open;
        /// <summary>
        /// Instance of object that hold information of the connection.
        /// </summary>
        public DbConnection Connection { get; }
        private bool Disposed { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectorType">Type of database connector, must be derived type of DbConnection</param>
        /// <param name="connectionString">Connection string to database</param>
        /// <exception cref="ArgumentNullException">Will throw argument null if connector type is null</exception>
        /// <exception cref="InvalidCastException">Will throw invalid cast if connector type is not a subclass of DbConnection</exception>
        public DatabaseConnector(Type connectorType, string connectionString)
        {
            if (connectorType == null) throw new ArgumentNullException(nameof(connectorType));
            if (!connectorType.IsSubclassOf(typeof(DbConnection)))
            {
                throw new InvalidCastException($"{connectorType.FullName} is not a derived type of DbConnection");
            }
            var connection = (DbConnection)Activator.CreateInstance(connectorType, connectionString);
            this.Connection = connection;
            this.Connection.Open();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">Instance of database connection</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DatabaseConnector(DbConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            connection.Open();
        }

        void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                Connection.Dispose();
            }
            Disposed = true;
        }
        /// <summary>
        /// Close the connection to the database.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
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
        public virtual IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false) where T : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.ParameterValue;
                    command.Parameters.Add(compatibleParameter);
                }
            }
            var cursor = command.ExecuteReader();
            var deferred = DataReaderBuilder<T>(cursor);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <returns>IEnumerable of dynamic object</returns>

        public virtual IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.ParameterValue;
                    command.Parameters.Add(compatibleParameter);
                }
            }
            var cursor = command.ExecuteReader();
            var deferred = DataReaderDynamicBuilder(cursor);
            if (buffered) deferred = deferred.AsList();
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

        public virtual T ExecuteScalar<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            T result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
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

        public virtual int ExecuteNonQuery(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            int result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
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
        /// <param name="buffered">Whether to buffered result in memory.</param>

        /// <returns>IEnumerable of POCO</returns>

        public virtual async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false) where T : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.ParameterValue;
                    command.Parameters.Add(compatibleParameter);
                }
            }
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var deferred = DataReaderBuilder<T>(cursor);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="transaction"></param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <returns>IEnumerable of dynamic object</returns>
        public virtual async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false)
        {
            using var command = Connection.CreateCommand();

            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.ParameterValue;
                    command.Parameters.Add(compatibleParameter);
                }
            }
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var deferred = DataReaderDynamicBuilder(cursor);
            if (buffered) deferred = deferred.AsList();
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

        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            T result;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
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

        public virtual async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            int result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
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

        public virtual object ExecuteScalar(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            object result;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
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

        public virtual async Task<object> ExecuteScalarAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            object result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction as DbTransaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter); command.Parameters.Add(parameter);
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
        public DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
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
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
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
        public async Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.ParameterValue;
                    command.Parameters.Add(compatibleParameter);
                }
            }
            DbDataReader cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var dataTable = new DataTable();
            dataTable.Load(cursor);
            return dataTable;
        }
        public virtual string CompatibleFunctionName(SqlFunction function) => function switch
        {
            SqlFunction.Length => "LEN",
            _ => throw new NotSupportedException(function.ToString())
        };
        private static IEnumerable<dynamic> DataReaderDynamicBuilder(DbDataReader reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return Shared.DataExtension.RowBuilder(reader);
                }
            }

        }
        private static IEnumerable<T> DataReaderBuilder<T>(DbDataReader reader) where T : class, new()
        {
            using (reader)
            {
                IDataMapper<T> converter = new Converter<T>(reader);
                while (reader.Read())
                {
                    yield return converter.GenerateObject();
                }
            }
        }

    }
#if NETSTANDARD2_1
    public partial class DatabaseConnector
    {
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, supported by async enumerable runtime.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>

        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public virtual async IAsyncEnumerable<T> ExecuteReaderAsyncEnumerable<T>(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text) where T : class, new()
        {
            await using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.ParameterValue;
                    command.Parameters.Add(compatibleParameter);
                }
            }
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var result = DataReaderBuilderAsync<T>(cursor);
            await foreach (var data in result)
            {
                yield return data;
            }
        }

        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object, supported by async enumerable runtime.
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="transaction"></param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <returns>IEnumerable of dynamic object</returns>
        public virtual async IAsyncEnumerable<dynamic> ExecuteReaderAsyncEnumerable(string sql, IEnumerable<DbParameterStruct>? parameters = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            await using var command = Connection.CreateCommand();

            command.CommandText = sql;
            command.Transaction = transaction as DbTransaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.ParameterValue;
                        command.Parameters.Add(compatibleParameter);
                    }
                }
            }
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var result = DataReaderDynamicBuilderAsync(cursor);
            await foreach (var data in result)
            {
                yield return data;
            }
        }

        private static async IAsyncEnumerable<dynamic> DataReaderDynamicBuilderAsync(DbDataReader reader)
        {
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    yield return Shared.DataExtension.RowBuilder(reader);
                }
            }

        }
        private static async IAsyncEnumerable<T> DataReaderBuilderAsync<T>(DbDataReader reader) where T : class, new()
        {
            using (reader)
            {
                IDataMapper<T> converter = new Converter<T>(reader);
                while (await reader.ReadAsync())
                {
                    yield return converter.GenerateObject();
                }
            }
        }
    }
#endif
}
