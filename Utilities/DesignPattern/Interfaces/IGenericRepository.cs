using System.Collections.Generic;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Contains required implementation on Repository template
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Select();

        void Insert(T obj);

        void Update(T obj);

        void Delete(T obj);
    }
}