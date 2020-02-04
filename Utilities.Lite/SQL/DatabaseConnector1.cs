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
    /// <typeparam name="TDatabaseConnection">Type of DbConnection</typeparam>
    /// <typeparam name="TParameterType">Type of DbParameter</typeparam>
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
    //#region CRUD operation
    //public partial class DatabaseConnector
    //{
    //    /// <summary>
    //    /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual IEnumerable<T> Query<T>(int? top = null, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var query = this.SelectQueryGenerate<T>(top);
    //        IEnumerable<T> result = ExecuteReader<T>(query, transaction: transaction);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Primary key of specific row</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    public virtual T Query<T>(object primaryKey, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var preparer = this.SelectQueryGenerate<T>(primaryKey);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        T result = ExecuteReader<T>(query, parameters, transaction: transaction).FirstOrDefault();
    //        return result;
    //    }
    //    /// <summary>
    //    /// Select first row from table.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    public virtual T QueryFirst<T>(IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var query = this.SelectQueryGenerate<T>(top: 1);
    //        T result = ExecuteReader<T>(query, transaction: transaction).FirstOrDefault();
    //        return result;
    //    }
    //    /// <summary>
    //    /// Select first row from table by using matched predicate.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    public virtual T QueryFirst<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var query = this.SelectQueryGenerate<T>(predicate, 1);
    //        T result = ExecuteReader(query.query, query.parameters, transaction).FirstOrDefault();
    //        return result;
    //    }
    //    /// <summary>
    //    /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj">Object to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    public virtual int Insert<T>(T obj, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        if (obj == null) return -1;
    //        var preparer = this.InsertQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = ExecuteNonQuery(query, parameters, transaction: transaction);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj">IEnumrable to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    public virtual int InsertMany<T>(IEnumerable<T> obj, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        if (obj == null || !obj.Any()) return -1;
    //        var preparer = this.InsertQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = ExecuteNonQuery(query, parameters, transaction: transaction);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj">Object to update.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an update.</returns>
    //    public virtual int Update<T>(T obj, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var preparer = this.UpdateQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var value = ExecuteNonQuery(query, parameters, transaction: transaction);
    //        return value;
    //    }

    //    /// <summary>
    //    /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj"></param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual int Delete<T>(T obj, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var preparer = this.DeleteQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = ExecuteNonQuery(query, parameters, transaction: transaction);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>IEnumerable of object</returns>
    //    public virtual async Task<IEnumerable<T>> QueryAsync<T>(int? top = null, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var query = this.SelectQueryGenerate<T>(top);
    //        var result = await ExecuteReaderAsync<T>(query, transaction: transaction).ConfigureAwait(false);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Primary key of specific row</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    public virtual async Task<T> QueryAsync<T>(object primaryKey, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var preparer = this.SelectQueryGenerate<T>(primaryKey);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = (await ExecuteReaderAsync<T>(query, parameters, transaction: transaction).ConfigureAwait(false)).FirstOrDefault();
    //        return result;
    //    }
    //    /// <summary>
    //    /// Select first row from table.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    public virtual async Task<T> QueryFirstAsync<T>(IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var query = this.SelectQueryGenerate<T>(top: 1);
    //        T result = (await ExecuteReaderAsync<T>(query, transaction: transaction).ConfigureAwait(false)).FirstOrDefault();
    //        return result;
    //    }
    //    /// <summary>
    //    /// Select first row from table by using matched predicate.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    public virtual async Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var query = this.SelectQueryGenerate<T>(predicate, 1);
    //        T result = (await ExecuteReaderAsync(query.query, query.parameters, transaction).ConfigureAwait(false)).FirstOrDefault();
    //        return result;
    //    }
    //    /// <summary>
    //    /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj">Object to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    public virtual async Task<int> InsertAsync<T>(T obj, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.InsertQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj">Object to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    public virtual async Task<int> InsertManyAsync<T>(IEnumerable<T> obj, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.InsertQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj">Object to update.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an update.</returns>
    //    public virtual async Task<int> UpdateAsync<T>(T obj, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var preparer = this.UpdateQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj"></param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual async Task<int> DeleteAsync<T>(T obj, IDbTransaction? transaction = null)
    //        where T : class, new()
    //    {
    //        var preparer = this.DeleteQueryGenerate<T>(obj);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Select data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.SelectQueryGenerate<T>(predicate, top);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = ExecuteReader<T>(query, parameters, transaction: transaction);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Delete data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual int Delete<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.DeleteQueryGenerate<T>(predicate);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        return ExecuteNonQuery(query, parameters, transaction: transaction);
    //    }

    //    /// <summary>
    //    /// Select data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual async Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.SelectQueryGenerate<T>(predicate, top);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        var result = await ExecuteReaderAsync<T>(query, parameters, transaction: transaction).ConfigureAwait(false);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Select data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.DeleteQueryGenerate<T>(predicate);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        return await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
    //    }

    //    /// <summary>
    //    /// Select data from table by using primary key
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Specified primary key.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual int Delete<T>(object primaryKey, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.DeleteQueryGenerate<T>(primaryKey);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        return ExecuteNonQuery(query, parameters, transaction: transaction);
    //    }

    //    /// <summary>
    //    /// Select data from table by using primary key
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Specified primary key.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    public virtual async Task<int> DeleteAsync<T>(object primaryKey, IDbTransaction? transaction = null) where T : class, new()
    //    {
    //        var preparer = this.DeleteQueryGenerate<T>(primaryKey);
    //        var query = preparer.query;
    //        var parameters = preparer.parameters;
    //        return await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
    //    }


    //    /// <summary>
    //    /// Returns rows count from specified table.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public virtual int Count<T>() where T : class
    //    {
    //        var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
    //        var query = $"SELECT COUNT(*) FROM {tableName}";
    //        var count = this.ExecuteScalar<int>(query);
    //        return count;
    //    }
    //    /// <summary>
    //    /// Returns rows count from specified table in an asynchronous manner.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public virtual async Task<int> CountAsync<T>() where T : class
    //    {
    //        var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
    //        var query = $"SELECT COUNT(*) FROM {tableName}";
    //        var count = await this.ExecuteScalarAsync<int>(query).ConfigureAwait(false);
    //        return count;
    //    }
    //    /// <summary>
    //    /// Provide converter to convert data type from CLR to underlying SQL type, default mapper is supported by SQL Server and can be override when necessary.
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    internal protected virtual string MapCLRTypeToSQLType(Type type)
    //    {
    //        if (type == typeof(string))
    //        {
    //            return "NVARCHAR(1024)";
    //        }
    //        else if (type == typeof(char) || type == typeof(char?))
    //        {
    //            return "NCHAR(1)";
    //        }
    //        else if (type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?))
    //        {
    //            return "SMALLINT";
    //        }
    //        else if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
    //        {
    //            return "INT";
    //        }
    //        else if (type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?))
    //        {
    //            return "BIGINT";
    //        }
    //        else if (type == typeof(float) || type == typeof(float?))
    //        {
    //            return "REAL";
    //        }
    //        else if (type == typeof(double) || type == typeof(double?))
    //        {
    //            return "FLOAT";
    //        }
    //        else if (type == typeof(bool) || type == typeof(bool?))
    //        {
    //            return "BIT";
    //        }
    //        else if (type == typeof(decimal) || type == typeof(decimal?))
    //        {
    //            return "MONEY";
    //        }
    //        else if (type == typeof(DateTime) || type == typeof(DateTime?))
    //        {
    //            return "DATETIME";
    //        }
    //        else if (type == typeof(Guid) || type == typeof(Guid?))
    //        {
    //            return "UNIQUEIDENTIFIER";
    //        }
    //        else if (type == typeof(byte) || type == typeof(byte?) || type == typeof(sbyte) || type == typeof(sbyte?))
    //        {
    //            return "TINYINT";
    //        }
    //        else if (type == typeof(byte[]))
    //        {
    //            return "VARBINARY";
    //        }
    //        else
    //        {
    //            throw new NotSupportedException($"Unable to map type {type.FullName} to {this.GetType().FullName} SQL Type");
    //        }
    //    }
    //}

    //#endregion
}
