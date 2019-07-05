﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Utilities.Shared;

namespace Utilities.SQL
{
    public abstract class DatabaseConnector<TDatabaseType, TParameter> : IDisposable
        where TDatabaseType : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        public TDatabaseType Connection { get; }
        public DatabaseConnector(string connectionString)
        {
            //connection = databaseType;
            //parameter = parameterType;
            Connection = new TDatabaseType()
            {
                ConnectionString = connectionString
            };
            Connection.Open();
        }
        public void Dispose()
        {
            Connection.Close();
        }
        public string ConnectionString => Connection.ConnectionString;
        public bool IsOpen => Connection != null && Connection.State == ConnectionState.Open;

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
        public IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where T : class, new()
        {
            return ExecuteReader(sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        public IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T ExecuteScalar<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public int ExecuteNonQuery(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="objectBuilder">How the POCO should build with each giving row of SqlDataReader</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of POCO</returns>
        /// <exception cref="Exception"/>
        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
         where T : class, new()
        {
            return await ExecuteReaderAsync<T>(sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
        }
        /// <summary>
        /// Execute SELECT SQL query and return IEnumerable of dynamic object
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns>IEnumerable of dynamic object</returns>
        /// <exception cref="Exception"/>
        public async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        /// <param name="connectionString">Connection string to database</param>
        /// <param name="sql">Any SELECT SQL that you want to perform with/without parameterized parameters (Do not directly put sql parameter in this parameter)</param>
        /// <param name="parameters">SQL parameters according to the sql parameter</param>
        /// <param name="commandType">Type of SQL Command</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        public IEnumerable<T> Select<T>()
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
        public T Select<T>(object primaryKey)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var primaryKeyAttribute = AttributeExtension.PrimaryKeyValidate(typeof(T).GetProperties());
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = ExecuteReader<T>(query, new[] {
                    new TParameter()
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
        public int Insert<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var result = ExecuteNonQuery(query.ToString(), kvMapper.Select(field => new TParameter()
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
        public int Update<T>(T obj)
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
            var parametersArray = parameters.Select(x => new TParameter()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameter() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            var value = ExecuteNonQuery(query, parametersArray);
            return value;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Delete<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var result = ExecuteNonQuery(query.ToString(), new[] {
                    new TParameter()
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
        public async Task<IEnumerable<T>> SelectAsync<T>()
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
        public async Task<T> SelectAsync<T>(object primaryKey)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var primaryKeyAttribute = AttributeExtension.PrimaryKeyValidate(typeof(T).GetProperties());
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = (await ExecuteReaderAsync<T>(query, new[] {
                    new TParameter()
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
        public async Task<int> InsertAsync<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var result = await ExecuteNonQueryAsync(query.ToString(), kvMapper.Select(field => new TParameter()
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
        public async Task<int> UpdateAsync<T>(T obj)
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
            var parametersArray = parameters.Select(x => new TParameter()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameter() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            var value = await ExecuteNonQueryAsync(query, parametersArray);
            return value;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(T obj)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var result = await ExecuteNonQueryAsync(query.ToString(), new[] {
                    new TParameter()
                    {
                        ParameterName = primaryKey.Name,
                        Value = primaryKey.GetValue(obj)
                    }
                });
            return result;
        }
    }
}