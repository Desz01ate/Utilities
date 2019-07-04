using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Shared;

namespace Utilities.SQL
{
    /// <summary>
    /// Provide wrapper access to Any Database with basic operation like ExecuteReader,ExecuteNonQuery and ExecuteScalar
    /// </summary>
    internal static class StaticDatabseConnector
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
        public static IEnumerable<T> ExecuteReader<T, TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
            where T : new()
            where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                List<T> result = new List<T>();
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static IEnumerable<T> ExecuteReader<T, TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
            where T : new()
            where TDatabaseType : DbConnection, new()
        {
            return ExecuteReader<T, TDatabaseType>(connectionString, sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
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
        public static IEnumerable<dynamic> ExecuteReader<TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                List<dynamic> result = new List<dynamic>();
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static T ExecuteScalar<T, TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                T result = default;
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static int ExecuteNonQuery<TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
            where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                int result = -1;
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static async Task<IEnumerable<T>> ExecuteReaderAsync<T, TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : new()
        where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                List<T> result = new List<T>();
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static async Task<IEnumerable<T>> ExecuteReaderAsync<T, TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text) where T : new()
        where TDatabaseType : DbConnection, new()
        {
            return await ExecuteReaderAsync<T, TDatabaseType>(connectionString, sql, parameters, (cursor) => Data.RowBuilder<T>(cursor), commandType);
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
        public static async Task<IEnumerable<dynamic>> ExecuteReaderAsync<TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                List<dynamic> result = new List<dynamic>();
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static async Task<T> ExecuteScalarAsync<T, TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                T result = default;
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
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
        public static async Task<int> ExecuteNonQueryAsync<TDatabaseType>(string connectionString, string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where TDatabaseType : DbConnection, new()
        {
            DbTransaction transaction = null;
            try
            {
                int result = -1;
                using (var connection = new TDatabaseType())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    transaction = connection.BeginTransaction();
                    using (var command = connection.CreateCommand())
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
                    connection.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
                throw e;
            }
        }

        #region Experimental CRUD
        public static IEnumerable<T> Select<T, TDatabaseType>(string connectionString)
            where T : class, new()
            where TDatabaseType : DbConnection, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var query = $"SELECT * FROM {tableName}";
            var result = ExecuteReader<T, TDatabaseType>(connectionString, query);
            return result;
        }
        public static T Select<T, TDatabaseType, TParameter>(string connectionString, object primaryKey)
            where T : class, new()
            where TDatabaseType : DbConnection, new()
            where TParameter : DbParameter, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var primaryKeyAttribute = AttributeExtension.PrimaryKeyValidate(typeof(T).GetProperties());
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = ExecuteReader<T, TDatabaseType>(connectionString, query, new[] {
                    new TParameter()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }).FirstOrDefault();
            return result;
        }
        public static int Insert<T, TDatabaseType, TParameter>(string connectionString, T obj)
            where T : class, new()
            where TDatabaseType : DbConnection, new()
            where TParameter : DbParameter, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var result = ExecuteNonQuery<TDatabaseType>(connectionString, query.ToString(), kvMapper.Select(field => new TParameter()
            {
                ParameterName = $"@{field.Key}",
                Value = field.Value
            }));
            return result;
        }
        public static int Update<T, TDatabaseType, TParameter>(string connectionString, T obj)
            where T : class, new()
            where TDatabaseType : DbConnection, new()
            where TParameter : DbParameter, new()
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
            var value = ExecuteNonQuery<TDatabaseType>(connectionString, query, parametersArray);
            return value;
        }
        public static int Delete<T, TDatabaseType, TParameter>(string connectionString, T obj)
            where T : class, new()
            where TDatabaseType : DbConnection, new()
            where TParameter : DbParameter, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var fields = typeof(T).GetProperties();
            var primaryKey = fields.PrimaryKeyValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var result = ExecuteNonQuery<TDatabaseType>(connectionString, query.ToString(), new[] {
                    new TParameter()
                    {
                        ParameterName = primaryKey.Name,
                        Value = primaryKey.GetValue(obj)
                    }
                });
            return result;
        }
        #endregion
    }
}
