using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.DesignPattern.UnitOfWork.Components
{
    /// <summary>
    /// Repository class designed for IDatabaseConnectorExtension.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDatabase"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class Repository<T, TDatabase, TParameter> : IGenericRepository<T>, IEnumerable<T>
        where T : class, new()
        where TDatabase : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        /// <summary>
        /// Instance of database connector.
        /// </summary>
        protected readonly IDatabaseConnectorExtension<TDatabase, TParameter> Database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="databaseConnector">Instance of DatabaseConnector.</param>
        public Repository(IDatabaseConnectorExtension<TDatabase, TParameter> databaseConnector)
        {
            Database = databaseConnector;
        }

        /// <summary>
        /// Delete data from repository.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual void Delete(T data)
        {
            Database.Delete(data);
        }

        /// <summary>
        /// Delete data from repository.
        /// </summary>
        /// <param name="key">Primary key of target object.</param>
        public virtual void Delete(object key)
        {
            Database.Delete<T>(key);
        }

        /// <summary>
        /// Delete data from repository in an asynchronous manner.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual async Task DeleteAsync(T data)
        {
            await Database.DeleteAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data from repository in an asynchronous manner.
        /// </summary>
        /// <param name="key">Primary key of target object.</param>
        public virtual async Task DeleteAsync(object key)
        {
            await Database.DeleteAsync<T>(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert data into repository.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual void Insert(T data)
        {
            Database.Insert(data);
        }

        /// <summary>
        /// Insert data into repository in an asynchronous manner.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual async Task InsertMultipleAsync(IEnumerable<T> data)
        {
            await Database.InsertMultipleAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert data into repository.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual void InsertMultiple(IEnumerable<T> data)
        {
            Database.InsertMultiple(data);
        }

        /// <summary>
        /// Insert data into repository in an asynchronous manner.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual async Task InsertAsync(T data)
        {
            await Database.InsertAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all data from repository.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> Query()
        {
            return Database.Query<T>();
        }

        /// <summary>
        /// Get data by specific condition from repository.
        /// </summary>
        /// <param name="predicate">Predicate condition.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return Database.Query<T>(predicate);
        }

        /// <summary>
        /// Get data from repository.
        /// </summary>
        /// <param name="key">Primary key of target object.</param>
        /// <returns></returns>
        public virtual T Query(object key)
        {
            return Database.Query<T>(key);
        }

        /// <summary>
        /// Get all data from repository in an asynchronous manner.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryAsync()
        {
            return await Database.QueryAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Get data by specific condition from repository in an asynchronous manner.
        /// </summary>
        /// <param name="predicate">Predicate condition.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate)
        {
            return await Database.QueryAsync<T>(predicate).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data from repository.
        /// </summary>
        /// <param name="key">Primary key of target object.</param>
        /// <returns></returns>
        public virtual async Task<T> QueryAsync(object key)
        {
            return await Database.QueryAsync<T>(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Update data in repository.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual void Update(T data)
        {
            Database.Update(data);
        }

        /// <summary>
        /// Update data in repository in an asynchronous manner.
        /// </summary>
        /// <param name="data">Generic object.</param>
        public virtual async Task UpdateAsync(T data)
        {
            await Database.UpdateAsync(data).ConfigureAwait(false);
        }
        /// <summary>
        /// Returns rows count from repository.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.Database.Count<T>();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Query(predicate);
        }
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<T> Take(int count)
        {
            return Database.Query<T>(top: count);
        }
        /// <summary>
        /// Get enumerator of data repository.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var data in Query())
            {
                yield return data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}