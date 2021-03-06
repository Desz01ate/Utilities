﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide extension set of operations on top of INoSQLConnector
    /// </summary>
    /// <typeparam name="TDatabaseType">DbConnection type</typeparam>
    /// <typeparam name="TParameter">DbParameter type</typeparam>
    public interface INoSQLConnectorExtension : INoSQLConnector
    {
        #region DML
        IEnumerable<T> Select<T>(int? top = null, Func<DbDataReader, T> dataBuilder = null) where T : class, new();
        IEnumerable<T> Select<T>(Expression<Func<T, bool>> predicate, int? top = null, Func<DbDataReader, T> dataBuilder = null) where T : class, new();
        T Select<T>(object primaryKey, Func<DbDataReader, T> dataBuilder = null) where T : class, new();
        int Insert<T>(T obj) where T : class, new();
        int Insert<T>(IEnumerable<T> obj) where T : class, new();
        int Update<T>(T obj) where T : class, new();
        //int Update<T>(T obj, Expression<Func<T, bool>> predicate,DbTransaction transaction = null) where T : class, new();
        int Delete<T>(T obj) where T : class, new();
        int Delete<T>(object primaryKey) where T : class, new();
        int Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        Task<IEnumerable<T>> SelectAsync<T>(int? top = null, Func<DbDataReader, T> dataBuilder = null) where T : class, new();
        Task<IEnumerable<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, Func<DbDataReader, T> dataBuilder = null) where T : class, new();
        Task<T> SelectAsync<T>(object primaryKey, Func<DbDataReader, T> dataBuilder = null) where T : class, new();
        Task<int> InsertAsync<T>(T obj) where T : class, new();
        Task<int> UpdateAsync<T>(T obj) where T : class, new();
        //Task<int> UpdateAsync<T>(T obj, Expression<Func<T, bool>> predicate,DbTransaction transaction = null) where T : class, new();
        Task<int> DeleteAsync<T>(T obj) where T : class, new();
        Task<int> DeleteAsync<T>(object primaryKey) where T : class, new();
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        #endregion
        #region DDL
        int CreateTable<T>() where T : class, new();
        int DROP_TABLE_USE_WITH_CAUTION<T>() where T : class, new();
        IEnumerable<Classes.TableSchema> GetSchema<T>() where T : class, new();
        string MapCLRTypeToSQLType(Type type);
        #endregion
    }
}
