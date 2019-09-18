using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Classes;
using Utilities.Enum;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Extension;
using Utilities.SQL.Translator;

namespace Utilities.SQL
{
    public partial class DatabaseConnector<TDatabaseConnection, TParameterType> : IDatabaseConnectorExtension<TDatabaseConnection, TParameterType>
    where TDatabaseConnection : DbConnection, new()
    where TParameterType : DbParameter, new()
    {

        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">Row builder template.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public IEnumerable<T> Select<T>(int? top = null, Func<DbDataReader, T> dataBuilder = null, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SelectQueryGenerate<T>(top);
            var query = preparer.query;
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = ExecuteReader<T>(query, transaction: transaction);
            }
            else
            {
                result = ExecuteReader<T>(query, null, objectBuilder: (cursor) => dataBuilder(cursor), transaction: transaction);
            }
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="dataBuilder">Row builder template.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public T Select<T>(object primaryKey, Func<DbDataReader, T> dataBuilder = null, DbTransaction transaction = null)
            where T : class, new()
        {
            //var type = typeof(T);
            //var tableName = type.TableNameAttributeValidate();
            //var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            //var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var preparer = SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            T result;
            if (dataBuilder == null)
            {
                result = ExecuteReader<T>(query, parameters, transaction: transaction).FirstOrDefault();
            }
            else
            {
                result = ExecuteReader<T>(query, parameters, objectBuilder: (cursor) => dataBuilder(cursor), transaction: transaction).FirstOrDefault();
            }
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public int Insert<T>(T obj, DbTransaction transaction = null)
            where T : class, new()
        {
            if (obj == null) return -1;
            var preparer = InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }
        /// <summary>
        /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">IEnumrable to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public int Insert<T>(IEnumerable<T> obj, DbTransaction transaction = null)
            where T : class, new()
        {
            if (obj == null || !obj.Any()) return -1;
            var preparer = InsertQueryGenerate(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an update.</returns>
        public int Update<T>(T obj, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var value = ExecuteNonQuery(query, parameters, transaction: transaction);
            return value;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public int Delete<T>(T obj, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteNonQuery(query, parameters);
            return result;
        }
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">Row builder template.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of object</returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(int? top = null, Func<DbDataReader, T> dataBuilder = null, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SelectQueryGenerate<T>(top);
            var query = preparer.query;
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = await ExecuteReaderAsync<T>(query, transaction: transaction).ConfigureAwait(false);
            }
            else
            {
                result = await ExecuteReaderAsync<T>(query, null, objectBuilder: (cursor) => dataBuilder(cursor), transaction: transaction).ConfigureAwait(false);
            }
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="dataBuilder">Row builder template.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public async Task<T> SelectAsync<T>(object primaryKey, Func<DbDataReader, T> dataBuilder = null, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            T result;
            if (dataBuilder == null)
            {
                result = (await ExecuteReaderAsync<T>(query, parameters, transaction: transaction).ConfigureAwait(false)).FirstOrDefault();
            }
            else
            {
                result = (await ExecuteReaderAsync<T>(query, parameters, objectBuilder: (cursor) => dataBuilder(cursor), transaction: transaction).ConfigureAwait(false)).FirstOrDefault();
            }
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public async Task<int> InsertAsync<T>(T obj, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an update.</returns>
        public async Task<int> UpdateAsync<T>(T obj, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(T obj, DbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">Row builder template.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> predicate, int? top = null, Func<DbDataReader, T> dataBuilder = null, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = ExecuteReader<T>(query, parameters, transaction: transaction);
            }
            else
            {
                result = ExecuteReader<T>(query, parameters, objectBuilder: (cursor) => dataBuilder(cursor), transaction: transaction);
            }
            return result;
        }

        /// <summary>
        /// Delete data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public int Delete<T>(Expression<Func<T, bool>> predicate, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">Row builder template.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, Func<DbDataReader, T> dataBuilder = null, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = await ExecuteReaderAsync<T>(query, parameters, transaction: transaction).ConfigureAwait(false);
            }
            else
            {
                result = await ExecuteReaderAsync<T>(query, parameters, objectBuilder: (cursor) => dataBuilder(cursor), transaction: transaction).ConfigureAwait(false);
            }
            return result;
        }
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public int Delete<T>(object primaryKey, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }
        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(object primaryKey, DbTransaction transaction = null) where T : class, new()
        {
            var preparer = DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Create table from given model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int CreateTable<T>() where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var fields = Data.GenerateSQLCreteFieldStatement<TDatabaseConnection, TParameterType, T>(this);
            var query = $@"CREATE TABLE {tableName}({string.Join(",", fields)})";
            var result = this.ExecuteNonQuery(query);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Use this method with CAUTION, THE ACTION CANNOT BE UNDONE!")]
        public int DROP_TABLE_USE_WITH_CAUTION<T>() where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var query = $@"DROP TABLE {tableName}";
            var result = this.ExecuteNonQuery(query);
            return result;
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) SelectQueryGenerate<T>(int? top = null) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var query = string.Format("SELECT {0} * FROM {1}", top.HasValue ? $"TOP({top.Value})" : "", tableName);
            return (query, null);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) SelectQueryGenerate<T>(Expression<Func<T, bool>> predicate, int? top = null) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = string.Format("SELECT {0} * FROM {1} WHERE {2}", top.HasValue ? $"TOP({top.Value})" : "", tableName, translateResult.Expression);
            return (query, translateResult.Parameters);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) SelectQueryGenerate<T>(object primaryKey) where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var parameter = new TParameterType()
            {
                ParameterName = primaryKeyAttribute.Name,
                Value = primaryKey
            };
            return (query, new[] { parameter });
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) InsertQueryGenerate<T>(T obj) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj, SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var parameters = kvMapper.Select(field => new TParameterType()
            {
                ParameterName = $"@{field.Key}",
                Value = field.Value
            });
            return (query, parameters);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) InsertQueryGenerate<T>(IEnumerable<T> obj) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var kvMapper = Shared.Data.CRUDDataMapping(obj.First(), SqlType.Insert);
            var query = new StringBuilder($@"INSERT INTO {tableName}({string.Join(",", kvMapper.Select(field => field.Key))}) VALUES");
            var values = new List<string>();
            var parameters = new List<TParameterType>();
            for (var idx = 0; idx < obj.Count(); idx++)
            {
                var o = obj.ElementAt(idx);
                var map = Shared.Data.CRUDDataMapping(o, SqlType.Insert);

                values.Add($"({ string.Join(",", map.Select(field => $"@{field.Key}{idx}"))})");
                parameters.AddRange(map.Select(field => new TParameterType()
                {
                    ParameterName = $"@{field.Key}{idx}",
                    Value = field.Value
                }));
            }
            var joinedValue = string.Join(",", values);
            query.Append(joinedValue);
            return (query.ToString(), parameters);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) UpdateQueryGenerate<T>(T obj) where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();
            var pkValue = primaryKey.GetValue(obj);
            var parameters = Shared.Data.CRUDDataMapping(obj, SqlType.Update);
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
            return (query, parametersArray);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) DeleteQueryGenerate<T>(T obj) where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var parameters = new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKey.Name,
                        Value = primaryKey.GetValue(obj)
                    } };
            return (query, parameters);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) DeleteQueryGenerate<T>(object primaryKey) where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"DELETE FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var parameters = new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                };
            return (query, parameters);
        }
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public (string query, IEnumerable<TParameterType> parameters) DeleteQueryGenerate<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = $@"DELETE FROM {tableName} WHERE {translateResult.Expression}";
            return (query, translateResult.Parameters);
        }
        /// <summary>
        /// Get table schema from current database connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<TableSchema> GetSchema<T>() where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            return Connection.GetTableSchema(tableName);
        }
        /// <summary>
        /// Provide converter to convert data type from CLR to underlying SQL type, default mapper is supported by SQL Server and can be override when neccessary.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string MapCLRTypeToSQLType(Type type)
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
    }
}
