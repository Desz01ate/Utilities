using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Shared;

namespace Utilities.SQLConnector
{
    public class DatabaseConnector<TDatabaseType, TParameter> : IDisposable
        where TDatabaseType : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        private TDatabaseType connection { get; }
        public DatabaseConnector(string connectionString)
        {
            //connection = databaseType;
            //parameter = parameterType;
            connection = new TDatabaseType()
            {
                ConnectionString = connectionString
            };
            connection.Open();
        }
        public void Dispose()
        {
            connection.Close();
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
        public IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<DbParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where T : class, new()
        {
            DbTransaction transaction = null;
            try
            {
                List<T> result = new List<T>();
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
        public IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        public IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = null;
            try
            {
                List<dynamic> result = new List<dynamic>();
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
        public T ExecuteScalar<T>(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = null;
            try
            {
                T result = default;
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
        public int ExecuteNonQuery(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = null;
            try
            {
                int result = -1;
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
        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<DbParameter> parameters, Func<DbDataReader, T> objectBuilder, System.Data.CommandType commandType = System.Data.CommandType.Text)
        where T : class, new()
        {
            DbTransaction transaction = null;
            try
            {
                List<T> result = new List<T>();
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
        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
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
        public async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = null;
            try
            {
                List<dynamic> result = new List<dynamic>();
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
        public async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = null;
            try
            {
                T result = default;
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
        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParameter> parameters = null, System.Data.CommandType commandType = System.Data.CommandType.Text)
        {
            DbTransaction transaction = null;
            try
            {
                int result = -1;
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
                return result;
            }
            catch (Exception e)
            {
                transaction?.Rollback();
                throw e;
            }
        }

        #region Experimental CRUD
        public IEnumerable<T> Select<T>()
            where T : class, new()
        {
            var tableName = typeof(T).TableNameValidate();
            var query = $"SELECT * FROM {tableName}";
            var result = ExecuteReader<T>(query);
            return result;
        }
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
        #endregion
    }
}
