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
    /// <typeparam name="TDatabaseType"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class Repository<T, TDatabase, TParameter> : IGenericRepository<T>
        where T : class, new()
        where TDatabase : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        protected readonly IDatabaseConnectorExtension<TDatabase, TParameter> _databaseConnector;
        public Repository(IDatabaseConnectorExtension<TDatabase, TParameter> databaseConnector)
        {
            _databaseConnector = databaseConnector;
        }
        public virtual void Delete(T obj)
        {
            _databaseConnector.Delete(obj);
        }

        public virtual void Delete(object key)
        {
            _databaseConnector.Delete<T>(key);
        }

        public virtual void Insert(T obj)
        {
            _databaseConnector.Insert(obj);
        }

        public virtual IEnumerable<T> Select()
        {
            return _databaseConnector.Select<T>();
        }

        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> predicate)
        {
            return _databaseConnector.Select<T>(predicate);
        }

        public virtual T Select(object key)
        {
            return _databaseConnector.Select<T>(key);
        }

        public virtual void Update(T obj)
        {
            _databaseConnector.Update(obj);
        }
    }
}
