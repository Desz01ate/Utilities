using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Extension;

namespace Utilities.SQL
{
    public partial class DatabaseConnector<TDatabaseConnection, TParameterType> : Abstract.DatabaseConnectorContractor, IDatabaseConnectorExtension<TDatabaseConnection, TParameterType>
    where TDatabaseConnection : DbConnection, new()
    where TParameterType : DbParameter, new()
    {
        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top">Specified TOP(n) rows.</param>

        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Query<T>(int? top = null, IDbTransaction transaction = null)
            where T : class, new()
        {
            var query = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(top);
            IEnumerable<T> result = ExecuteReader<T>(query, transaction: transaction);
            return result;
        }

        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>

        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public virtual T Query<T>(object primaryKey, IDbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            T result = ExecuteReader<T>(query, parameters, transaction: transaction).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public virtual T QueryFirst<T>(IDbTransaction transaction = null) where T : class, new()
        {
            var query = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(top: 1);
            T result = ExecuteReader<T>(query, transaction: transaction).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table by using matched predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public virtual T QueryFirst<T>(Expression<Func<T, bool>> predicate, IDbTransaction transaction = null) where T : class, new()
        {
            var query = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(this, predicate, 1);
            T result = ExecuteReader(query.query, query.parameters, transaction).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public virtual int Insert<T>(T obj, IDbTransaction transaction = null)
            where T : class, new()
        {
            if (obj == null) return -1;
            var preparer = SqlQueryExtension.InsertQueryGenerate<T, TParameterType>(obj);
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
        public virtual int InsertMultiple<T>(IEnumerable<T> obj, IDbTransaction transaction = null)
            where T : class, new()
        {
            if (obj == null || !obj.Any()) return -1;
            var preparer = SqlQueryExtension.InsertQueryGenerate<T, TParameterType>(obj);
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
        public virtual int Update<T>(T obj, IDbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SqlQueryExtension.UpdateQueryGenerate<T, TParameterType>(obj);
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
        public virtual int Delete<T>(T obj, IDbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SqlQueryExtension.DeleteQueryGenerate<T, TParameterType>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteNonQuery(query, parameters, transaction: transaction);
            return result;
        }

        /// <summary>
        /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top">Specified TOP(n) rows.</param>

        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>IEnumerable of object</returns>
        public virtual async Task<IEnumerable<T>> QueryAsync<T>(int? top = null, IDbTransaction transaction = null)
            where T : class, new()
        {
            var query = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(top);
            var result = await ExecuteReaderAsync<T>(query, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Primary key of specific row</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public virtual async Task<T> QueryAsync<T>(object primaryKey, IDbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = (await ExecuteReaderAsync<T>(query, parameters, transaction: transaction).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public virtual async Task<T> QueryFirstAsync<T>(IDbTransaction transaction = null) where T : class, new()
        {
            var query = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(top: 1);
            T result = (await ExecuteReaderAsync<T>(query, transaction: transaction).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Select first row from table by using matched predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Object of given class</returns>
        public virtual async Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> predicate, IDbTransaction transaction = null) where T : class, new()
        {
            var query = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(this, predicate, 1);
            T result = (await ExecuteReaderAsync(query.query, query.parameters, transaction).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public virtual async Task<int> InsertAsync<T>(T obj, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.InsertQueryGenerate<T, TParameterType>(obj);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to insert.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns>Affected row after an insert.</returns>
        public virtual async Task<int> InsertMultipleAsync<T>(IEnumerable<T> obj, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.InsertQueryGenerate<T, TParameterType>(obj);
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
        public virtual async Task<int> UpdateAsync<T>(T obj, IDbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SqlQueryExtension.UpdateQueryGenerate<T, TParameterType>(obj);
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
        public virtual async Task<int> DeleteAsync<T>(T obj, IDbTransaction transaction = null)
            where T : class, new()
        {
            var preparer = SqlQueryExtension.DeleteQueryGenerate<T, TParameterType>(obj);
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

        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(this, predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = ExecuteReader<T>(query, parameters, transaction: transaction);
            return result;
        }

        /// <summary>
        /// Delete data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual int Delete<T>(Expression<Func<T, bool>> predicate, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.DeleteQueryGenerate<T, TParameterType>(this, predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return ExecuteNonQuery(query, parameters, transaction: transaction);
        }

        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="top">Specified TOP(n) rows.</param>

        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.SelectQueryGenerate<T, TParameterType>(this, predicate, top);
            var query = preparer.query;
            var parameters = preparer.parameters;
            var result = await ExecuteReaderAsync<T>(query, parameters, transaction: transaction).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Select data from table by using matched predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Predicate of data in LINQ manner</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.DeleteQueryGenerate<T, TParameterType>(this, predicate);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
        }

        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual int Delete<T>(object primaryKey, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.DeleteQueryGenerate<T, TParameterType>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return ExecuteNonQuery(query, parameters, transaction: transaction);
        }

        /// <summary>
        /// Select data from table by using primary key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">Specified primary key.</param>
        /// <param name="transaction">Transaction for current execution.</param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync<T>(object primaryKey, IDbTransaction transaction = null) where T : class, new()
        {
            var preparer = SqlQueryExtension.DeleteQueryGenerate<T, TParameterType>(primaryKey);
            var query = preparer.query;
            var parameters = preparer.parameters;
            return await ExecuteNonQueryAsync(query, parameters, transaction: transaction).ConfigureAwait(false);
        }


        /// <summary>
        /// Returns rows count from specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual int Count<T>() where T : class
        {
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var query = $"SELECT COUNT(*) FROM {tableName}";
            var count = this.ExecuteScalar<int>(query);
            return count;
        }
        /// <summary>
        /// Returns rows count from specified table in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<int> CountAsync<T>() where T : class
        {
            var tableName = AttributeExtension.TableNameAttributeValidate(typeof(T));
            var query = $"SELECT COUNT(*) FROM {tableName}";
            var count = await this.ExecuteScalarAsync<int>(query).ConfigureAwait(false);
            return count;
        }
        /// <summary>
        /// Provide converter to convert data type from CLR to underlying SQL type, default mapper is supported by SQL Server and can be override when necessary.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal protected override string MapCLRTypeToSQLType(Type type)
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

        /// <summary>
        /// Create table from given model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual int CreateTable<T>() where T : class, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var fields = DataExtension.GenerateSQLCreteFieldStatement<T>(this);
            var query = $@"CREATE TABLE {tableName}({string.Join(",", fields)})";
            return this.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Drop specific table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //[Obsolete("Use this method with CAUTION, THE ACTION CANNOT BE UNDONE!")]
        //public virtual int DROP_TABLE_USE_WITH_CAUTION<T>() where T : class, new()
        //{
        //    var tableName = typeof(T).TableNameAttributeValidate();
        //    var query = $@"DROP TABLE {tableName}";
        //    return this.ExecuteNonQuery(query);
        //}

    }
}