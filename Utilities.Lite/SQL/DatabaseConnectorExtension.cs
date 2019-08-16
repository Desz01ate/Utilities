using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Utilities.Shared;
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
        /// <returns>IEnumerable of object</returns>
        public IEnumerable<T> Select<T>(int? top = null, Func<DbDataReader, T> dataBuilder = null)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var query = string.Format("SELECT {0} * FROM {1}", top.HasValue ? $"TOP({top.Value})" : "", tableName);
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = ExecuteReader<T>(query);
            }
            else
            {
                result = ExecuteReader<T>(query, null, objectBuilder: (cursor) => dataBuilder(cursor));
            }
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <returns>Object of given class</returns>
        public T Select<T>(object primaryKey, Func<DbDataReader, T> dataBuilder = null)
            where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            T result;
            if (dataBuilder == null)
            {
                result = ExecuteReader<T>(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }).FirstOrDefault();
            }
            else
            {
                result = ExecuteReader<T>(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }, objectBuilder: (cursor) => dataBuilder(cursor)).FirstOrDefault();
            }
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
            var tableName = typeof(T).TableNameAttributeValidate();
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
        public int Update<T>(T obj)
            where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();
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
        public int Delete<T>(T obj)
            where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();

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
        public async Task<IEnumerable<T>> SelectAsync<T>(int? top = null, Func<DbDataReader, T> dataBuilder = null)
            where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var query = string.Format("SELECT {0} * FROM {1}", top.HasValue ? $"TOP({top.Value})" : "", tableName);
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = await ExecuteReaderAsync<T>(query);
            }
            else
            {
                result = await ExecuteReaderAsync<T>(query, null, objectBuilder: (cursor) => dataBuilder(cursor));
            }
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <returns>Object of given class</returns>
        public async Task<T> SelectAsync<T>(object primaryKey, Func<DbDataReader, T> dataBuilder = null)
            where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            T result;
            if (dataBuilder == null)
            {
                result = (await ExecuteReaderAsync<T>(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                })).FirstOrDefault();
            }
            else
            {
                result = (await ExecuteReaderAsync<T>(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }, objectBuilder: (cursor) => dataBuilder(cursor))).FirstOrDefault();
            }
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Affected row after an insert.</returns>
        public async Task<int> InsertAsync<T>(T obj) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
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
        public async Task<int> UpdateAsync<T>(T obj)
            where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();
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
        public async Task<int> DeleteAsync<T>(T obj)
            where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();

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
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <returns></returns>
        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> predicate, int? top = null, Func<DbDataReader, T> dataBuilder = null) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = string.Format("SELECT {0} * FROM {1} WHERE {2}", top.HasValue ? $"TOP({top.Value})" : "", tableName, translateResult.Expression);
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = ExecuteReader<T>(query, translateResult.Parameters);
            }
            else
            {
                result = ExecuteReader<T>(query, translateResult.Parameters, objectBuilder: (cursor) => dataBuilder(cursor));
            }
            return result;
        }
        /// <summary>
        /// Update data to specific table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <returns></returns>
        //public int Update<T>(T obj, Expression<Func<T, bool>> predicate) where T : class, new()
        //{
        //    var tableName = typeof(T).TableNameValidate();
        //    var fields = typeof(T).GetProperties();
        //    var primaryKey = fields.PrimaryKeyValidate();
        //    var pkValue = primaryKey.GetValue(obj);
        //    var parameters = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Update);
        //    parameters.Remove(primaryKey.Name);
        //    var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
        //    var translatorResult = translator.Translate(predicate);
        //    var query = $@"UPDATE {tableName} SET
        //                       {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
        //                   WHERE {translatorResult.expression}";

        //    var parametersArray = parameters.Select(x => new TParameterType()
        //    {
        //        ParameterName = $"@{x.Key}",
        //        Value = x.Value
        //    }).ToList();
        //    parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
        //    var value = ExecuteNonQuery(query, parametersArray.Concat(translatorResult.parameters));
        //    return value;
        //}
        /// <summary>
        /// Delete data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <returns></returns>
        public int Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = $@"DELETE FROM {tableName} WHERE {translateResult.Expression}";
            var result = ExecuteNonQuery(query, translateResult.Parameters);
            return result;
        }
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, Func<DbDataReader, T> dataBuilder = null) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = string.Format("SELECT {0} * FROM {1} WHERE {2}", top.HasValue ? $"TOP({top.Value})" : "", tableName, translateResult.Expression);
            IEnumerable<T> result;
            if (dataBuilder == null)
            {
                result = await ExecuteReaderAsync<T>(query, translateResult.Parameters);
            }
            else
            {
                result = await ExecuteReaderAsync<T>(query, translateResult.Parameters, objectBuilder: (cursor) => dataBuilder(cursor));
            }
            return result;
        }
        /// <summary>
        /// Update data to specific table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <returns></returns>
        //public async Task<int> UpdateAsync<T>(T obj, Expression<Func<T, bool>> predicate) where T : class, new()
        //{
        //    var tableName = typeof(T).TableNameValidate();
        //    var fields = typeof(T).GetProperties();
        //    var primaryKey = fields.PrimaryKeyValidate();
        //    var pkValue = primaryKey.GetValue(obj);
        //    var parameters = Shared.Data.CRUDDataMapping(obj, Enumerables.SqlType.Update);
        //    parameters.Remove(primaryKey.Name);
        //    var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
        //    var translatorResult = translator.Translate(predicate);
        //    var query = $@"UPDATE {tableName} SET
        //                       {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
        //                   WHERE " + translatorResult;

        //    var parametersArray = parameters.Select(x => new TParameterType()
        //    {
        //        ParameterName = $"@{x.Key}",
        //        Value = x.Value
        //    }).ToList();
        //    parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
        //    var value = await ExecuteNonQueryAsync(query, parametersArray.Concat(translatorResult.parameters));
        //    return value;
        //}
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var baseStatement = $@"DELETE FROM {tableName} WHERE ";
            var translator = new ExpressionTranslator<T, TParameterType>(SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var result = await ExecuteNonQueryAsync(baseStatement + translateResult.Expression, translateResult.Parameters);
            return result;
        }

        public int Delete<T>(object primaryKey) where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"DELETE FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = this.ExecuteNonQuery(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }, CommandType.Text);
            return result;
        }

        public async Task<int> DeleteAsync<T>(object primaryKey) where T : class, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"DELETE FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var result = await this.ExecuteNonQueryAsync(query, new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                }, CommandType.Text);
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
            var fields = Data.GenerateSQLCreteFieldStatement<T>();
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
    }
}
