﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    //public partial interface IDatabaseConnector
    //{
    //    #region DML
    //    /// <summary>
    //    /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    IEnumerable<T> Query<T>(int? top = null, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Primary key of specific row</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    T Query<T>(object primaryKey, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select first row from table.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    T QueryFirst<T>(IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select one first from table by using matched predicate.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    T QueryFirst<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data">Object to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    int Insert<T>(T data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data">IEnumrable to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    int InsertMany<T>(IEnumerable<T> data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data">Object to update.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an update.</returns>
    //    int Update<T>(T data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj"></param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    int Delete<T>(T data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select data from table by using primary key
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Specified primary key.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    int Delete<T>(object primaryKey, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Delete data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    int Delete<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select all rows from table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>IEnumerable of object</returns>
    //    Task<IEnumerable<T>> QueryAsync<T>(int? top = null, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="top">Specified TOP(n) rows.</param>

    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate, int? top = null, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select one row from table from given primary key (primary key can be set by [PrimaryKey] attribute, table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Primary key of specific row</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    Task<T> QueryAsync<T>(object primaryKey, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select first row from table.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    Task<T> QueryFirstAsync<T>(IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select first row from table by using matched predicate.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Object of given class</returns>
    //    Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Insert row into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data">Object to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    Task<int> InsertAsync<T>(T data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Insert rows into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data">Object to insert.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an insert.</returns>
    //    Task<int> InsertManyAsync<T>(IEnumerable<T> data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Update specific object into table (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data">Object to update.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns>Affected row after an update.</returns>
    //    Task<int> UpdateAsync<T>(T data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Delete given object from table by inference of [PrimaryKey] attribute. (table name is a class name or specific [Table] attribute, an attribute has higher priority).
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="data"></param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    Task<int> DeleteAsync<T>(T data, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select data from table by using primary key
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="primaryKey">Specified primary key.</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    Task<int> DeleteAsync<T>(object primaryKey, IDbTransaction? transaction = null) where T : class, new();
    //    /// <summary>
    //    /// Select data from table by using matched predicate
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="predicate">Predicate of data in LINQ manner</param>
    //    /// <param name="transaction">Transaction for current execution.</param>
    //    /// <returns></returns>
    //    Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate, IDbTransaction? transaction = null) where T : class, new();

    //    #endregion DML

    //    #region DDL
    //    ///// <summary>
    //    ///// Create table from given model
    //    ///// </summary>
    //    ///// <typeparam name="T"></typeparam>
    //    ///// <returns></returns>
    //    //int CreateTable<T>() where T : class, new();
    //    #endregion DDL
    //    /// <summary>
    //    /// Returns rows count from specified table.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    int Count<T>() where T : class;
    //    /// <summary>
    //    /// Returns rows count from specified table in an asynchronous manner.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    Task<int> CountAsync<T>() where T : class;
    //}
}
