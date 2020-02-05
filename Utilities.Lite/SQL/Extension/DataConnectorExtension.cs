using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Utilities.Shared;

namespace Utilities.SQL.Extension
{
    /// <summary>
    /// Provide extension set on CRUD operation for IDatabaseConnector
    /// </summary>
    public static partial class DataConnectorExtension
    {
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this IDatabaseConnector connector, int? top = null, IDbTransaction? transaction = null, bool buffered = false)
            where T : class, new()
        {
            var query = connector.SelectQueryGenerate<T>(top);
            IEnumerable<T> result = connector.ExecuteReader<T>(query, transaction: transaction, buffered: buffered);
            return result;
        }

        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>Object of given class</returns>
        public static T Query<T>(this IDatabaseConnector connector, object primaryKey, IDbTransaction? transaction = null, bool buffered = false)
            where T : class, new()
        {
            var preparer = connector.SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            T result = connector.ExecuteReader<T>(query, parameters, transaction: transaction, buffered: buffered).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>Object of given class</returns>
        public static T QueryFirst<T>(this IDatabaseConnector connector, IDbTransaction? transaction = null, bool buffered = false) where T : class, new()
        {
            var query = connector.SelectQueryGenerate<T>(top: 1);
            T result = connector.ExecuteReader<T>(query, transaction: transaction, buffered: buffered).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table by using matched predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>Object of given class</returns>
        public static T QueryFirst<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null, bool buffered = false) where T : class, new()
        {
            var query = connector.SelectQueryGenerate<T>(predicate, 1);
            T result = connector.ExecuteReader(query.query, query.parameters, transaction, buffered: buffered).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public static int Insert<T>(this IDatabaseConnector connector, T obj, IDbTransaction? transaction = null)
            where T : class, new()
        {
            if (obj == null) return -1;
            var preparer = connector.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = connector.ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }

        /// <summary>
        /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj">IEnumrable to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public static int InsertMany<T>(this IDatabaseConnector connector, IEnumerable<T> obj, IDbTransaction? transaction = null)
            where T : class, new()
        {
            if (obj == null || !obj.Any()) return -1;
            var preparer = connector.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = connector.ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }

        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj">Object to update.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an update.</returns>
        public static int Update<T>(this IDatabaseConnector connector, T obj, IDbTransaction? transaction = null)
            where T : class, new()
        {
            var preparer = connector.UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var value = connector.ExecuteNonQuery(query, parameters, transaction: transaction);
            return value;
        }

        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static int Delete<T>(this IDatabaseConnector connector, T obj, IDbTransaction? transaction = null)
            where T : class, new()
        {
            var preparer = connector.DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = connector.ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }

        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>IEnumerable of object</returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDatabaseConnector connector, int? top = null, IDbTransaction? transaction = null, bool buffered = false)
            where T : class, new()
        {
            var query = connector.SelectQueryGenerate<T>(top);
            var result = await connector.ExecuteReaderAsync<T>(query, transaction: transaction, buffered: buffered).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>Object of given class</returns>
        public static async Task<T> QueryAsync<T>(this IDatabaseConnector connector, object primaryKey, IDbTransaction? transaction = null, bool buffered = false)
            where T : class, new()
        {
            var preparer = connector.SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = (await connector.ExecuteReaderAsync<T>(query, parameters, transaction: transaction, buffered: buffered).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>Object of given class</returns>
        public static async Task<T> QueryFirstAsync<T>(this IDatabaseConnector connector, IDbTransaction? transaction = null, bool buffered = false) where T : class, new()
        {
            var query = connector.SelectQueryGenerate<T>(top: 1);
            T result = (await connector.ExecuteReaderAsync<T>(query, transaction: transaction, buffered: buffered).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table by using matched predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns>Object of given class</returns>
        public static async Task<T> QueryFirstAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null, bool buffered = false) where T : class, new()
        {
            var query = connector.SelectQueryGenerate<T>(predicate, 1);
            T result = (await connector.ExecuteReaderAsync(query.query, query.parameters, transaction, buffered: buffered).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public static async Task<int> InsertAsync<T>(this IDatabaseConnector connector, T obj, IDbTransaction? transaction = null) where T : class, new()
        {
            var preparer = connector.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await connector.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public static async Task<int> InsertManyAsync<T>(this IDatabaseConnector connector, IEnumerable<T> obj, IDbTransaction? transaction = null) where T : class, new()
        {
            var preparer = connector.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await connector.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj">Object to update.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an update.</returns>
        public static async Task<int> UpdateAsync<T>(this IDatabaseConnector connector, T obj, IDbTransaction? transaction = null)
            where T : class, new()
        {
            var preparer = connector.UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await connector.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="obj"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<T>(this IDatabaseConnector connector, T obj, IDbTransaction? transaction = null)
            where T : class, new()
        {
            var preparer = connector.DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await connector.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction? transaction = null, bool buffered = false) where T : class, new()
        {
            var preparer = connector.SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = connector.ExecuteReader<T>(query, parameters, transaction: transaction, buffered: buffered);
            return result;
        }

        /// <summary>
        /// Delete data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static int Delete<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new()
        {
            var preparer = connector.DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return connector.ExecuteNonQuery(query, parameters, transaction: transaction);
        }

        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction? transaction = null, bool buffered = false) where T : class, new()
        {
            var preparer = connector.SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await connector.ExecuteReaderAsync<T>(query, parameters, transaction: transaction, buffered: buffered).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new()
        {
            var preparer = connector.DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return await connector.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
        }

        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static int Delete<T>(this IDatabaseConnector connector, object primaryKey, IDbTransaction? transaction = null) where T : class, new()
        {
            var preparer = connector.DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return connector.ExecuteNonQuery(query, parameters, transaction: transaction);
        }

        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<T>(this IDatabaseConnector connector, object primaryKey, IDbTransaction? transaction = null) where T : class, new()
        {
            var preparer = connector.DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return await connector.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
        }


        /// <summary>
        /// Returns rows count from specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Count<T>(this IDatabaseConnector connector) where T : class
        {
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var query = $"SELECT COUNT(*) FROM {tableName}";
            var count = connector.ExecuteScalar<int>(query);
            return count;
        }
        /// <summary>
        /// Returns rows count from specified table in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this IDatabaseConnector connector) where T : class
        {
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var query = $"SELECT COUNT(*) FROM {tableName}";
            var count = await connector.ExecuteScalarAsync<int>(query).ConfigureAwait(false);
            return count;
        }
    }
}
