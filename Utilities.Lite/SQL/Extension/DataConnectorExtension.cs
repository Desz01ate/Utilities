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
using Utilities.SQL.Abstracts;
using Utilities.SQL.Translator;

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
        public static IEnumerable<T> Query<T>(this IDatabaseConnector connector, int? top = null, DbTransaction? transaction = null, bool buffered = true)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>(top);
            IEnumerable<T> result = con.ExecuteReader<T>(query, transaction: transaction, buffered: buffered);
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
        public static T Query<T>(this IDatabaseConnector connector, object primaryKey, DbTransaction? transaction = null, bool buffered = true)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            T result = con.ExecuteReader<T>(query, parameters, transaction: transaction, buffered: buffered).FirstOrDefault();
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
        public static T QueryFirst<T>(this IDatabaseConnector connector, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>(top: 1);
            T result = con.ExecuteReader<T>(query, transaction: transaction, buffered: buffered).FirstOrDefault();
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
        public static T QueryFirst<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>(predicate, 1);
            T result = con.ExecuteReader(query.query, query.parameters, transaction, buffered: buffered).FirstOrDefault();
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
        public static int Insert<T>(this IDatabaseConnector connector, T obj, DbTransaction? transaction = null)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            if (obj == null) return -1;
            var preparer = con.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = con.ExecuteNonQuery(query, parameters, transaction: transaction);
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
        public static int InsertMany<T>(this IDatabaseConnector connector, IEnumerable<T> obj, DbTransaction? transaction = null)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            if (obj == null || !obj.Any()) return -1;
            var preparer = con.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = con.ExecuteNonQuery(query, parameters, transaction: transaction);
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
        public static int Update<T>(this IDatabaseConnector connector, T obj, DbTransaction? transaction = null)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var value = con.ExecuteNonQuery(query, parameters, transaction: transaction);
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
        public static int Delete<T>(this IDatabaseConnector connector, T obj, DbTransaction? transaction = null)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = con.ExecuteNonQuery(query, parameters, transaction: transaction);
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
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDatabaseConnector connector, int? top = null, DbTransaction? transaction = null, bool buffered = true)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>(top);
            var result = await con.ExecuteReaderAsync<T>(query, transaction: transaction, buffered: buffered).ConfigureAwait(false);
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
        public static async Task<T> QueryAsync<T>(this IDatabaseConnector connector, object primaryKey, DbTransaction? transaction = null, bool buffered = true)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = (await con.ExecuteReaderAsync<T>(query, parameters, transaction: transaction, buffered: buffered).ConfigureAwait(false)).FirstOrDefault();
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
        public static async Task<T> QueryFirstAsync<T>(this IDatabaseConnector connector, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>(top: 1);
            T result = (await con.ExecuteReaderAsync<T>(query, transaction: transaction, buffered: buffered).ConfigureAwait(false)).FirstOrDefault();
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
        public static async Task<T> QueryFirstAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>(predicate, 1);
            T result = (await con.ExecuteReaderAsync(query.query, query.parameters, transaction, buffered: buffered).ConfigureAwait(false)).FirstOrDefault();
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
        public static async Task<int> InsertAsync<T>(this IDatabaseConnector connector, T obj, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await con.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
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
        public static async Task<int> InsertManyAsync<T>(this IDatabaseConnector connector, IEnumerable<T> obj, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await con.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
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
        public static async Task<int> UpdateAsync<T>(this IDatabaseConnector connector, T obj, DbTransaction? transaction = null)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await con.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
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
        public static async Task<int> DeleteAsync<T>(this IDatabaseConnector connector, T obj, DbTransaction? transaction = null)
            where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await con.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
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
        public static IEnumerable<T> Query<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, int? top = null, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = con.ExecuteReader<T>(query, parameters, transaction: transaction, buffered: buffered);
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
        public static int Delete<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return con.ExecuteNonQuery(query, parameters, transaction: transaction);
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
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, int? top = null, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await con.ExecuteReaderAsync<T>(query, parameters, transaction: transaction, buffered: buffered).ConfigureAwait(false);
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
        public static async Task<int> DeleteAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return await con.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
        }

        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static int Delete<T>(this IDatabaseConnector connector, object primaryKey, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return con.ExecuteNonQuery(query, parameters, transaction: transaction);
        }

        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<T>(this IDatabaseConnector connector, object primaryKey, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var preparer = con.DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return await con.ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
        }


        /// <summary>
        /// Returns rows count from specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Count<T>(this IDatabaseConnector connector) where T : class
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var query = $"SELECT COUNT(*) FROM {tableName}";
            var count = con.ExecuteScalar(query);
            var countAsString = count.ToString();
            return int.Parse(countAsString);
        }
        /// <summary>
        /// Returns rows count from specified table in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this IDatabaseConnector connector) where T : class
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var query = $"SELECT COUNT(*) FROM {tableName}";
            var count = await con.ExecuteScalarAsync(query).ConfigureAwait(false);
            var countAsString = count.ToString();
            return int.Parse(countAsString);
        }
        /// <summary>
        /// Returns rows count on specific condition from specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Count<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate) where T : class
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var exprTranslator = new ExpressionTranslator<T>(con.CompatibleFunctionName);
            var translateResult = exprTranslator.Translate(predicate);
            var query = $"SELECT COUNT(*) FROM {tableName} WHERE {translateResult.Expression}";
            var count = con.ExecuteScalar(query, translateResult.Parameters);
            var countAsString = count.ToString();
            return int.Parse(countAsString);
        }
        /// <summary>
        /// Returns rows count on specific condition from specified table in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<int> CountAsync<T>(this IDatabaseConnector connector, Expression<Func<T, bool>> predicate) where T : class
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var exprTranslator = new ExpressionTranslator<T>(con.CompatibleFunctionName);
            var translateResult = exprTranslator.Translate(predicate);
            var query = $"SELECT COUNT(*) FROM {tableName} WHERE {translateResult.Expression}";
            var count = await con.ExecuteScalarAsync(query, translateResult.Parameters).ConfigureAwait(false);
            var countAsString = count.ToString();
            return int.Parse(countAsString);
        }
    }
    public static partial class DataConnectorExtension
    {
        /// <summary>
        /// Select rows from table by skipping rows by specified offset and take limit rows (SQL Server syntax).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="offset">The amount of rows to be offset (skip).</param>
        /// <param name="limit">The amount of rows to be take.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryOffset<T>(this IDatabaseConnector connector, int offset, int limit, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>();
            var primaryKey = AttributeExtension.PrimaryKeyAttributeValidate(typeof(T));
            if (primaryKey == null) throw new Exception("You must specified [PrimaryKey] attribute in order to use QueryOffset without specified column.");
            var orderBy = primaryKey.Name;
            var queryAppender = new StringBuilder(query);
            queryAppender.AppendLine($" ORDER BY {orderBy} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
            var queryAppendString = queryAppender.ToString();
            var result = con.ExecuteReader<T>(queryAppendString, transaction: transaction, buffered: buffered);
            return result;
        }
        /// <summary>
        /// Select rows from table by skipping rows by specified offset and take limit rows (SQL Server syntax).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="orderBy">Order by column.</param>
        /// <param name="offset">The amount of rows to be offset (skip).</param>
        /// <param name="limit">The amount of rows to be take.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryOffset<T>(this IDatabaseConnector connector, string orderBy, int offset, int limit, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>();
            var queryAppender = new StringBuilder(query);
            queryAppender.AppendLine($" ORDER BY {orderBy} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
            var result = con.ExecuteReader<T>(queryAppender.ToString(), transaction: transaction, buffered: buffered);
            return result;
        }
        /// <summary>
        /// Select rows from table by skipping rows by specified offset and take limit rows (SQL Server syntax).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="offset">The amount of rows to be offset (skip).</param>
        /// <param name="limit">The amount of rows to be take.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> QueryOffsetAsync<T>(this IDatabaseConnector connector, int offset, int limit, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>();
            var primaryKey = AttributeExtension.PrimaryKeyAttributeValidate(typeof(T));
            if (primaryKey == null) throw new Exception("You must specified [PrimaryKey] attribute in order to use QueryOffset without specified column.");
            var orderBy = primaryKey.Name;
            var queryAppender = new StringBuilder(query);
            queryAppender.AppendLine($" ORDER BY {orderBy} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
            var result = await con.ExecuteReaderAsync<T>(queryAppender.ToString(), transaction: transaction, buffered: buffered).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Select rows from table by skipping rows by specified offset and take limit rows (SQL Server syntax).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="orderBy">Order by column.</param>
        /// <param name="offset">The amount of rows to be offset (skip).</param>
        /// <param name="limit">The amount of rows to be take.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <param name="buffered">Whether the data should be cached in memory.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> QueryOffsetAsync<T>(this IDatabaseConnector connector, string orderBy, int offset, int limit, DbTransaction? transaction = null, bool buffered = true) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.SelectQueryGenerate<T>();
            var queryAppender = new StringBuilder(query);
            queryAppender.AppendLine($" ORDER BY {orderBy} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
            var result = await con.ExecuteReaderAsync<T>(queryAppender.ToString(), transaction: transaction, buffered: buffered).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Create table from model object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int CreateTable<T>(this IDatabaseConnector connector, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.GenerateCreateTableStatement<T>();
            var result = con.ExecuteNonQuery(query, transaction: transaction);
            return result;
        }
        /// <summary>
        /// Create table from model object in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static async Task<int> CreateTableAsync<T>(this IDatabaseConnector connector, DbTransaction? transaction = null) where T : class, new()
        {
            var con = connector as DatabaseConnectorBase;
            if (con == null) throw new InvalidCastException($"{connector.GetType().FullName} cannot be use with this extension (expected to get instance of {typeof(DatabaseConnectorBase).FullName}");
            var query = con.GenerateCreateTableStatement<T>();
            var result = await con.ExecuteNonQueryAsync(query, transaction: transaction).ConfigureAwait(false);
            return result;
        }
    }
}
