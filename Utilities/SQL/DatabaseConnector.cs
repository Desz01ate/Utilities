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
    public class DatabaseConnector<TDatabaseConnection, TParameterType> : IDatabaseConnector<TDatabaseConnection, TParameterType>
        where TDatabaseConnection : DbConnection, new()
        where TParameterType : DbParameter, new()
    {
        /// <summary>
        /// Instance of object that hold information of the connection.
        /// </summary>
        public TDatabaseConnection Connection { get; }
        private bool disposed { get; set; }
        protected Dictionary<SqlFunction, string> SQLFunctionConfiguration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string for database</param>
        public DatabaseConnector(string connectionString)
        {
            //connection = databaseType;
            //parameter = parameterType;
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
            if (disposed) return;
            if (disposing)
            {
                Connection.Close();
            }
            disposed = true;
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
        /// Connection string of this object.
        /// </summary>
        public virtual string ConnectionString => Connection.ConnectionString;
        /// <summary>
        /// Determine wheter the connection is open or not.
        /// </summary>
        public virtual bool IsOpen => Connection != null && Connection.State == ConnectionState.Open;

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
            DbTransaction transaction = null;
            try
            {
                List<T> result = new List<T>();
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                List<dynamic> result = new List<dynamic>();
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                T result = default;
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                int result = -1;
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                List<T> result = new List<T>();
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                List<dynamic> result = new List<dynamic>();
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                T result = default;
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
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
            DbTransaction transaction = null;
            try
            {
                int result = -1;
                transaction = Connection.BeginTransaction();
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
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
                throw e;
            }
        }

        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable of object</returns>
        public virtual IEnumerable<T> Select<T>()
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var query = $"SELECT * FROM {tableName}";
            var result = ExecuteReader<T>(query);
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <returns>Object of given class</returns>
        public virtual T Select<T>(object primaryKey)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var primaryKeyAttribute = AttributeExtension.PrimaryKeyValidate(typeof(T).GetProperties());
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = ExecuteReader<T>(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Affected row after an insert.</returns>
        public virtual int Insert<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var result = ExecuteNonQuery(query.ToString(), kvMapper.Select(field => new TParameterType()
            {
                ParameterName = $"@{field.Key}",
                Value = field.Value
            }));
            return result;
        }
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <returns>Affected row after an update.</returns>
        public virtual int Update<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();
            var pkValue = primaryKey.GetValue(obj);
            var parameters = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Update);
            parameters.Remove(primaryKey.Name);
            var query = $@"UPDATE {tableName} SET
                               {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
                                WHERE 
                               {primaryKey.Name} = @{primaryKey.Name}";
            var parametersArray = parameters.Select(x => new TParameterType()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            var value = ExecuteNonQuery(query, parametersArray);
            return value;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int Delete<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var result = ExecuteNonQuery(query.ToString(), new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKey.Name,
                        Value = primaryKey.GetValue(obj)
                    }
                });
            return result;
        }
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable of object</returns>
        public virtual async Task<IEnumerable<T>> SelectAsync<T>()
    where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var query = $"SELECT * FROM {tableName}";
            var result = await ExecuteReaderAsync<T>(query);
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <returns>Object of given class</returns>
        public virtual async Task<T> SelectAsync<T>(object primaryKey)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var primaryKeyAttribute = AttributeExtension.PrimaryKeyValidate(typeof(T).GetProperties());
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = (await ExecuteReaderAsync<T>(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                })).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Affected row after an insert.</returns>
        public virtual async Task<int> InsertAsync<T>(T obj) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var result = await ExecuteNonQueryAsync(query.ToString(), kvMapper.Select(field => new TParameterType()
            {
                ParameterName = $"@{field.Key}",
                Value = field.Value
            }));
            return result;
        }
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <returns>Affected row after an update.</returns>
        public virtual async Task<int> UpdateAsync<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();
            var pkValue = primaryKey.GetValue(obj);
            var parameters = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Update);
            parameters.Remove(primaryKey.Name);
            var query = $@"UPDATE {tableName} SET
                               {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
                                WHERE 
                               {primaryKey.Name} = @{primaryKey.Name}";
            var parametersArray = parameters.Select(x => new TParameterType()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            var value = await ExecuteNonQueryAsync(query, parametersArray);
            return value;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var result = await ExecuteNonQueryAsync(query.ToString(), new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKey.Name,
                        Value = primaryKey.GetValue(obj)
                    }
                });
            return result;
        }

        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var translator = new ExpressionTranslator(SQLFunctionConfiguration);
            var translatorResult = translator.Translate(predicate);
            var query = $@"SELECT * FROM {tableName} WHERE {translatorResult}";
            var result = ExecuteReader<T>(query);
            return result;
        }

        public int Update<T>(T obj, Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();
            var pkValue = primaryKey.GetValue(obj);
            var parameters = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Update);
            parameters.Remove(primaryKey.Name);
            var translator = new ExpressionTranslator(SQLFunctionConfiguration);
            var translatorResult = translator.Translate(predicate);
            var query = $@"UPDATE {tableName} SET
                               {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
                           WHERE {translatorResult}";

            var parametersArray = parameters.Select(x => new TParameterType()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            var value = ExecuteNonQuery(query, parametersArray);
            return value;
        }

        public int Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var translator = new ExpressionTranslator(SQLFunctionConfiguration);
            var translatorResult = translator.Translate(predicate);
            var query = $@"DELETE FROM {tableName} WHERE {translatorResult}";
            var result = ExecuteNonQuery(query);
            return result;
        }

        public async Task<IEnumerable<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var translator = new ExpressionTranslator(SQLFunctionConfiguration);
            var translatorResult = translator.Translate(predicate);
            var query = $@"SELECT * FROM {tableName} WHERE {translatorResult}";
            var result = await ExecuteReaderAsync<T>(query);
            return result;
        }

        public async Task<int> UpdateAsync<T>(T obj, Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();
            var pkValue = primaryKey.GetValue(obj);
            var parameters = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Update);
            parameters.Remove(primaryKey.Name);
            var translator = new ExpressionTranslator(SQLFunctionConfiguration);
            var translatorResult = translator.Translate(predicate);
            var query = $@"UPDATE {tableName} SET
                               {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
                           WHERE " + translatorResult;

            var parametersArray = parameters.Select(x => new TParameterType()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            var value = await ExecuteNonQueryAsync(query, parametersArray);
            return value;
        }

        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var baseStatement = $@"DELETE FROM {tableName} WHERE ";
            var translator = new ExpressionTranslator(SQLFunctionConfiguration);
            var translatorResult = translator.Translate(predicate);
            var result = await ExecuteNonQueryAsync(baseStatement + translatorResult);
            return result;
        }
    }
}
