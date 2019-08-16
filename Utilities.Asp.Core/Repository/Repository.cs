using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using Utilities.Asp.Core.Repository.Interfaces;
using Utilities.Interfaces;
using Utilities.SQL;

namespace Utilities.Asp.Core.Repository
{
    /// <summary>
    /// Example implementation of IGenericRepository with dependency injection of DAL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDatabaseType"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class Repository<T> : IGenericRepository<T>
        where T : class, new()
    {
        protected readonly DatabaseConnector<SqlConnection, SqlParameter> _databaseConnector;
        public Repository(DatabaseConnector<SqlConnection, SqlParameter> databaseConnector)
        {
            _databaseConnector = databaseConnector;
        }
        public virtual void Delete(T obj)
        {
            _databaseConnector.Delete(obj);
        }

        public virtual void Delete(object key)
        {
            _databaseConnector.Delete(key);
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
