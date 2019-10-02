#if !NET45
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;
using Utilities.Interfaces;
using Dapper;
using Utilities.SQL.Extension;
using System.Linq;
using System.Linq.Expressions;
using Utilities.Shared;

namespace Utilities.SQL
{
    /// <summary>
    /// Connector which internally combine the original DatabaseConnector and override some methods to take advantage of Dapper for high performance scenario.
    /// </summary>
    /// <typeparam name="TDatabaseConnection"></typeparam>
    /// <typeparam name="TParameterType"></typeparam>
    public class DapperConnector<TDatabaseConnection, TParameterType> : DatabaseConnector<TDatabaseConnection, TParameterType>
         where TDatabaseConnection : DbConnection, new()
         where TParameterType : DbParameter, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public DapperConnector(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return this.Connection.Execute(sql, parameters.ToDapperParameters(), commandType: commandType, transaction: transaction);
        }
        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return await this.Connection.ExecuteAsync(sql, parameters.ToDapperParameters(), commandType: commandType, transaction: transaction).ConfigureAwait(false);
        }
        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of dynamic.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return this.Connection.Query(sql, parameters.ToDapperParameters(), transaction, commandType: commandType);
        }
        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of T.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return this.Connection.Query<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType);
        }
        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of dynamic.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return await this.Connection.QueryAsync(sql, parameters.ToDapperParameters(), transaction, commandType: commandType).ConfigureAwait(false);
        }
        /// <summary>
        /// Execute parameterized SQL and return an IEnumerable of T.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return await this.Connection.QueryAsync<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType).ConfigureAwait(false);

        }
        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override T ExecuteScalar<T>(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return this.Connection.ExecuteScalar<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType);
        }
        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<TParameterType> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            return await this.Connection.ExecuteScalarAsync<T>(sql, parameters.ToDapperParameters(), transaction, commandType: commandType).ConfigureAwait(false);
        }
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">This parameter had no effect in Dapper environment.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override IEnumerable<T> Select<T>(int? top = null, DbTransaction transaction = null)
        {
            var preparer = SelectQueryGenerate<T>(top);
            var query = preparer.query;
            return this.Connection.Query<T>(query, transaction: transaction);
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="dataBuilder">This parameter had no effect in Dapper environment.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public override T Select<T>(object primaryKey, DbTransaction transaction = null)
        {
            var preparer = SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = this.Connection.QueryFirstOrDefault<T>(query, parameters, transaction);
            return result;
        }
        /// <summary>
        /// Select data from table using predicate (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate condition of data</param>
        /// <param name="dataBuilder">This parameter had no effect in Dapper environment.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public override IEnumerable<T> Select<T>(Expression<Func<T, bool>> predicate, int? top = null, DbTransaction transaction = null)
        {
            var preparer = SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = this.Connection.Query<T>(query, parameters, transaction);
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public override int Insert<T>(T obj, DbTransaction transaction = null)
        {
            if (obj == null) return -1;
            var preparer = InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = this.Connection.Execute(query, parameters, transaction);
            return result;
        }
        /// <summary>
        /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">IEnumrable to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public override int Insert<T>(IEnumerable<T> obj, DbTransaction transaction = null)
        {
            if (obj == null || !obj.Any()) return -1;
            var preparer = InsertQueryGenerate(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = this.Connection.Execute(query, parameters, transaction);
            return result;
        }
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an update.</returns>
        public override int Update<T>(T obj, DbTransaction transaction = null)
        {
            var preparer = UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var value = this.Connection.Execute(query, parameters, transaction);
            return value;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override int Delete<T>(T obj, DbTransaction transaction = null)
        {
            var preparer = DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = this.Connection.Execute(query, parameters, transaction);
            return result;
        }
        /// <summary>
        /// Delete given object from table by given primary key. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of target row.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override int Delete<T>(object primaryKey, DbTransaction transaction = null)
        {
            var preparer = DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            return this.Connection.Execute(query, parameters, transaction);
        }
        /// <summary>
        /// Delete data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override int Delete<T>(Expression<Func<T, bool>> predicate, DbTransaction transaction = null)
        {
            var preparer = DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            return this.Connection.Execute(query, parameters, transaction);
        }
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">This parameter had no effect in Dapper environment.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of object</returns>
        public override async Task<IEnumerable<T>> SelectAsync<T>(int? top = null, DbTransaction transaction = null)
        {
            var preparer = SelectQueryGenerate<T>(top);
            var query = preparer.query;
            var result = await this.Connection.QueryAsync<T>(query, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="dataBuilder">This parameter had no effect in Dapper environment.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public override async Task<T> SelectAsync<T>(object primaryKey, DbTransaction transaction = null)
        {
            var preparer = SelectQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            T result = await this.Connection.QueryFirstOrDefaultAsync<T>(query, parameters, transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="top">Specified TOP(n) rows.</param>
        /// <param name="dataBuilder">This parameter had no effect in Dapper environment.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override async Task<IEnumerable<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, DbTransaction transaction = null)
        {
            var preparer = SelectQueryGenerate<T>(predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            IEnumerable<T> result = await this.Connection.QueryAsync<T>(query, parameters, transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public override async Task<int> InsertAsync<T>(T obj, DbTransaction transaction = null)
        {
            var preparer = InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = await this.Connection.ExecuteAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public override async Task<int> InsertAsync<T>(IEnumerable<T> obj, DbTransaction transaction = null)
        {
            var preparer = InsertQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = await this.Connection.ExecuteAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to update.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an update.</returns>
        public override async Task<int> UpdateAsync<T>(T obj, DbTransaction transaction = null)
        {
            var preparer = UpdateQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = await this.Connection.ExecuteAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override async Task<int> DeleteAsync<T>(T obj, DbTransaction transaction = null)
        {
            var preparer = DeleteQueryGenerate<T>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = await this.Connection.ExecuteAsync(query, parameters, transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Delete given object from table by given primary key. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of target row.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override async Task<int> DeleteAsync<T>(object primaryKey, DbTransaction transaction = null)
        {
            var preparer = DeleteQueryGenerate<T>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = await this.Connection.ExecuteAsync(query, parameters, transaction).ConfigureAwait(false);
            return result;
        }
        /// <summary>
        /// Delete data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public override async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate, DbTransaction transaction = null)
        {
            var preparer = DeleteQueryGenerate<T>(predicate);
            var query = preparer.query;
            var parameters = preparer.parameters.ToDapperParameters();
            var result = await this.Connection.ExecuteAsync(query, parameters, transaction).ConfigureAwait(false);
            return result;
        }

    }
}
#endif