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
        protected readonly DatabaseConnector<SqlConnection, SqlParameter> DatabaseConnector;
        public Repository(DatabaseConnector<SqlConnection, SqlParameter> databaseConnector)
        {
            DatabaseConnector = databaseConnector;
        }
        public virtual void Delete(T obj)
        {
            DatabaseConnector.Delete(obj);
        }

        public virtual void Delete(object key)
        {
            DatabaseConnector.Delete(key);
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
