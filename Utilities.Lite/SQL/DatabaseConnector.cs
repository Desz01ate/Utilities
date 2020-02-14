using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Classes;
using Utilities.Enum;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Abstracts;
using Utilities.SQL.Extension;
using Utilities.SQL.Translator;

using static Utilities.SQL.Events.ExecutionInterceptor;

namespace Utilities.SQL
{
    /// <summary>
    /// Default implementation class of DatabaseConnectorProvider, provide an necessary implement for IDatabaseConnector.
    /// </summary>
    public partial class DatabaseConnector : DatabaseConnectorBase
    {
        /// <summary>
        /// Connection string of this object.
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                return Connection.ConnectionString;
            }
            set
            {
                Connection.ConnectionString = value;
            }
        }
        /// <summary>
        /// Determine whether the connection is open or not.
        /// </summary>
        public override bool IsOpen => Connection.State == ConnectionState.Open;
        /// <summary>
        /// Instance of object that hold information of the connection.
        /// </summary>
        public override DbConnection Connection { get; }
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
        public override IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = command.ExecuteReader();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
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

        public override IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = command.ExecuteReader();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
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

        public override T ExecuteScalar<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            T result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter);
                    }
                }
                this.OnQueryExecuting?.Invoke(sql, parameters);
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

        public override int ExecuteNonQuery(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            int result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter);
                    }
                }
                this.OnQueryExecuting?.Invoke(sql, parameters);
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

        public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
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
        public override async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
        {
            using var command = Connection.CreateCommand();

            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
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

        public override async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            T result;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter);
                    }
                }
                this.OnQueryExecuting?.Invoke(sql, parameters);
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

        public override async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            int result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter);
                    }
                }
                this.OnQueryExecuting?.Invoke(sql, parameters);
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

        public override object ExecuteScalar(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            object result;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter);
                    }
                }
                this.OnQueryExecuting?.Invoke(sql, parameters);
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

        public override async Task<object> ExecuteScalarAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            object result;
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;
                command.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter); command.Parameters.Add(parameter);
                    }
                }
                this.OnQueryExecuting?.Invoke(sql, parameters);
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
        public override DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = command.ExecuteReader();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var dataTable = new DataTable();
            dataTable.Load(cursor);
            return dataTable;
        }

        /// <summary>
        /// Execute SELECT SQL query and return DataTable in an asynchronous manner
        /// </summary>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override async Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var dataTable = new DataTable();
            dataTable.Load(cursor);
            return dataTable;
        }
        /// <summary>
        /// Provide function to convert from internal support SQL function to compatible SQL function (default support SQL Server).
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        internal protected override string CompatibleFunctionName(SqlFunction function) => function switch
        {
            SqlFunction.Length => "LEN",
            SqlFunction.Sum => "SUM",
            _ => throw new NotSupportedException(function.ToString())
        };
        /// <summary>
        /// Provide function to convert from CLR type to compatible SQL type (default support SQL Server).
        /// </summary>
        /// <param name="type">CLR type</param>
        /// <returns></returns>
        internal protected override string CompatibleSQLType(Type type)
        {
            if (type == typeof(string))
            {
                return "NVARCHAR(1024)";
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                return "NCHAR(1)";
            }
            else if (type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?))
            {
                return "SMALLINT";
            }
            else if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
            {
                return "INT";
            }
            else if (type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?))
            {
                return "BIGINT";
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return "REAL";
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return "FLOAT";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return "BIT";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "MONEY";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "DATETIME";
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "UNIQUEIDENTIFIER";
            }
            else if (type == typeof(byte) || type == typeof(byte?) || type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return "TINYINT";
            }
            else if (type == typeof(byte[]))
            {
                return "VARBINARY";
            }
            else
            {
                throw new NotSupportedException($"Unable to map type {type.FullName} to {this.GetType().FullName} SQL Type");
            }
        }
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
        public virtual async IAsyncEnumerable<T> ExecuteReaderAsyncEnumerable<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text) where T : class, new()
        {
            await using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
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

        public virtual async IAsyncEnumerable<dynamic> ExecuteReaderAsyncEnumerable(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text)
        {
            await using var command = Connection.CreateCommand();

            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    {
                        var compatibleParameter = command.CreateParameter();
                        compatibleParameter.ParameterName = parameter.ParameterName;
                        compatibleParameter.Value = parameter.Value;
                        compatibleParameter.Direction = parameter.Direction;
                        parameter.SetBindingRedirection(compatibleParameter);
                        command.Parameters.Add(compatibleParameter);
                    }
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync().ConfigureAwait(false);
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
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
    public partial class DatabaseConnector
    {
        /// <summary>
        /// The event that will trigger when query is ready to execute.
        /// </summary>
        public event OnQueryExecutingEventHandler? OnQueryExecuting;
        /// <summary>
        /// The event that will trigger when query is executed.
        /// </summary>
        public event OnQueryExecutedEventHandler? OnqueryExecuted;
    }
    public partial class DatabaseConnector
    {
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, this overload is suitable when you perform SELECT-JOIN query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="map">Map function.</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public IEnumerable<T1> ExecuteReader<T1, T2>(string sql, Func<T1, T2, T1> map, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
            where T1 : class, new()
            where T2 : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = command.ExecuteReader();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var deferred = DataReaderBuilder(cursor, map);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, this overload is suitable when you perform SELECT-JOIN query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="map">Map function.</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public IEnumerable<T1> ExecuteReader<T1, T2, T3>(string sql, Func<T1, T2, T3, T1> map, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = command.ExecuteReader();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var deferred = DataReaderBuilder(cursor, map);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, this overload is suitable when you perform SELECT-JOIN query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="map">Map function.</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public IEnumerable<T1> ExecuteReader<T1, T2, T3, T4>(string sql, Func<T1, T2, T3, T4, T1> map, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = command.ExecuteReader();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var deferred = DataReaderBuilder(cursor, map);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, this overload is suitable when you perform SELECT-JOIN query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="map">Map function.</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public async Task<IEnumerable<T1>> ExecuteReaderAsync<T1, T2>(string sql, Func<T1, T2, T1> map, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
            where T1 : class, new()
            where T2 : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var deferred = DataReaderBuilder(cursor, map);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, this overload is suitable when you perform SELECT-JOIN query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="map">Map function.</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public async Task<IEnumerable<T1>> ExecuteReaderAsync<T1, T2, T3>(string sql, Func<T1, T2, T3, T1> map, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var deferred = DataReaderBuilder(cursor, map);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of specified POCO that is matching with the query columns, this overload is suitable when you perform SELECT-JOIN query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter).</param>
        /// <param name="map">Map function.</param>
        /// <param name="parameters">SQL parameters according to the sql parameter.</param>
        /// <param name="commandType">Type of SQL Command.</param>
        /// <param name="buffered">Whether to buffered result in memory.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of POCO</returns>
        public async Task<IEnumerable<T1>> ExecuteReaderAsync<T1, T2, T3, T4>(string sql, Func<T1, T2, T3, T4, T1> map, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = true)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var compatibleParameter = command.CreateParameter();
                    compatibleParameter.ParameterName = parameter.ParameterName;
                    compatibleParameter.Value = parameter.Value;
                    compatibleParameter.Direction = parameter.Direction;
                    parameter.SetBindingRedirection(compatibleParameter);
                    command.Parameters.Add(compatibleParameter);
                }
            }
            this.OnQueryExecuting?.Invoke(sql, parameters);
            var cursor = await command.ExecuteReaderAsync();
            this.OnqueryExecuted?.Invoke(cursor.RecordsAffected);
            var deferred = DataReaderBuilder(cursor, map);
            if (buffered) deferred = deferred.AsList();
            return deferred;
        }
        private static IEnumerable<T1> DataReaderBuilder<T1, T2>(DbDataReader reader, Func<T1, T2, T1> map)
        where T1 : class, new()
        where T2 : class, new()
        {
            using (reader)
            {
                IDataMapper<T1> converter = new Converter<T1>(reader);
                IDataMapper<T2> childConverter = new Converter<T2>(reader);
                while (reader.Read())
                {
                    yield return map(converter.GenerateObject(), childConverter.GenerateObject());
                }
            }
        }
        private static IEnumerable<T1> DataReaderBuilder<T1, T2, T3>(DbDataReader reader, Func<T1, T2, T3, T1> map)
    where T1 : class, new()
    where T2 : class, new()
            where T3 : class, new()
        {
            using (reader)
            {
                IDataMapper<T1> converter = new Converter<T1>(reader);
                IDataMapper<T2> converter2 = new Converter<T2>(reader);
                IDataMapper<T3> converter3 = new Converter<T3>(reader);

                while (reader.Read())
                {
                    yield return map(converter.GenerateObject(), converter2.GenerateObject(), converter3.GenerateObject());
                }
            }
        }
        private static IEnumerable<T1> DataReaderBuilder<T1, T2, T3, T4>(DbDataReader reader, Func<T1, T2, T3, T4, T1> map)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            using (reader)
            {
                IDataMapper<T1> converter = new Converter<T1>(reader);
                IDataMapper<T2> converter2 = new Converter<T2>(reader);
                IDataMapper<T3> converter3 = new Converter<T3>(reader);
                IDataMapper<T4> converter4 = new Converter<T4>(reader);
                while (reader.Read())
                {
                    yield return map(converter.GenerateObject(), converter2.GenerateObject(), converter3.GenerateObject(), converter4.GenerateObject());
                }
            }
        }
    }
    public partial class DatabaseConnector
    {
        /// <summary>
        /// Gets the name of the current database after a connection is opened, or the database
        /// name specified in the connection string before the connection is opened.
        /// </summary>
        public override string Database => Connection.Database;
        /// <summary>
        /// The name of the database server to which to connect. The default value is an
        /// empty string. 
        /// </summary>
        public override string DataSource => Connection.DataSource;
        /// <summary>
        /// Gets a string that represents the version of the server to which the object is
        /// connected. 
        /// </summary>
        public override string ServerVersion => Connection.ServerVersion;
        /// <summary>
        /// Gets a string that describes the state of the connection.
        /// </summary>
        public override ConnectionState State => Connection.State;
        /// <summary>
        /// Starts a database transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return Connection.BeginTransaction(isolationLevel);
        }
        /// <summary>
        /// Changes the current database for an open connection.
        /// </summary>
        /// <param name="databaseName"></param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
        public override void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }
        /// <summary>
        /// Closes the connection to the database. This is the preferred method of closing
        /// any open connection.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void Close()
        {
            Connection?.Close();
        }
        /// <summary>
        /// Creates and returns a System.Data.Common.DbCommand object associated with the
        /// current connection.
        /// </summary>
        protected override DbCommand CreateDbCommand()
        {
            return Connection.CreateCommand();
        }
        /// <summary>
        /// Opens a database connection with the settings specified by the System.Data.Common.DbConnection.ConnectionString.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void Open()
        {
            Connection?.Open();
        }
#if NETSTANDARD2_1
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
        public override Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            return base.ChangeDatabaseAsync(databaseName, cancellationToken);
        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return base.OpenAsync(cancellationToken);
        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override Task CloseAsync()
        {
            return base.CloseAsync();
        }
#endif
    }
}
