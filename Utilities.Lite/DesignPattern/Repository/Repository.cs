using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using Utilities.Interfaces;
using Utilities.SQL;

namespace Utilities.DesignPattern
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
        protected readonly IDatabaseConnectorExtension<TDatabase, TParameter> DatabaseConnector;
        public Repository(IDatabaseConnectorExtension<TDatabase, TParameter> databaseConnector)
        {
            DatabaseConnector = databaseConnector;
        }
        public virtual void Delete(T obj)
        {
            DatabaseConnector.Delete(obj);
        }

        public virtual void Delete(object key)
        {
            DatabaseConnector.Delete<T>(key);
        }

        public virtual void Insert(T obj)
        {
            DatabaseConnector.Insert(obj);
        }

        public virtual IEnumerable<T> Select()
        {
            return DatabaseConnector.Select<T>();
        }

        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> predicate)
        {
            return DatabaseConnector.Select<T>(predicate);
        }

        public virtual T Select(object key)
        {
            return DatabaseConnector.Select<T>(key);
        }

        public virtual void Update(T obj)
        {
            DatabaseConnector.Update(obj);
        }
    }
}
