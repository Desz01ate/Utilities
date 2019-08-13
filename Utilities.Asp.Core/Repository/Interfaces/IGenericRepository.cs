using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.Asp.Core.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Select();
        IEnumerable<T> Select(Expression<Func<T, bool>> predicate);
        T Select(object key);
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        void Delete(object key);
    }
}
