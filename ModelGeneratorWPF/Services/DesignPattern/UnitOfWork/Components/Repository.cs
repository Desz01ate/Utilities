using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using Utilities.Interfaces;

namespace Utilities.DesignPattern.UnitOfWork.Components
{
    /// <summary>
    /// Example implementation of IGenericRepository with dependency injection of DAL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDatabase"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class Repository<T, TDatabase, TParameter> : IGenericRepository<T>
        where T : class, new()
        where TDatabase : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        /// <summary>
        /// Instance of database connector.
        /// </summary>
        protected readonly IDatabaseConnectorExtension<TDatabase, TParameter> DatabaseConnector;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="databaseConnector">Instance of DatabaseConnector.</param>
        public Repository(IDatabaseConnectorExtension<TDatabase, TParameter> databaseConnector)
        {
            DatabaseConnector = databaseConnector;
        }
        /// <summary>
        /// Delete data from repository.
        /// </summary>
        /// <param name="obj">Generic object.</param>
        public virtual void Delete(T obj)
        {
            DatabaseConnector.Delete(obj);
        }
        /// <summary>
        /// Delete data from repository.
        /// </summary>
        /// <param name="key">Primary key of target object.</param>
        public virtual void Delete(object key)
        {
            DatabaseConnector.Delete<T>(key);
        }
        /// <summary>
        /// Insert data into repository.
        /// </summary>
        /// <param name="obj">Generic object.</param>
        public virtual void Insert(T obj)
        {
            DatabaseConnector.Insert(obj);
        }
        /// <summary>
        /// Get all data from repository.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> Select()
        {
            return DatabaseConnector.Select<T>();
        }
        /// <summary>
        /// Get data by specific condition from repository.
        /// </summary>
        /// <param name="predicate">Predicate condition.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> predicate)
        {
            return DatabaseConnector.Select<T>(predicate);
        }
        /// <summary>
        /// Get data from repository.
        /// </summary>
        /// <param name="key">Primary key of target object.</param>
        /// <returns></returns>
        public virtual T Select(object key)
        {
            return DatabaseConnector.Select<T>(key);
        }
        /// <summary>
        /// Update data in repository.
        /// </summary>
        /// <param name="obj">Generic object.</param>
        public virtual void Update(T obj)
        {
            DatabaseConnector.Update(obj);
        }
    }
}
