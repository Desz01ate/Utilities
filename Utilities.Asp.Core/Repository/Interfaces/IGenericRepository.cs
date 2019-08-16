using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.Asp.Core.Repository.Interfaces
{
    /// <summary>
    /// Contains required implementation on Repository template
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Select();
        T Select(object key);
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        void Delete(object key);
    }
}
