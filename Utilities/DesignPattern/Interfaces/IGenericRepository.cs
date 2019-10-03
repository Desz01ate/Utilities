using System.Collections.Generic;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Contains required implementation on Repository template
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Select data from repository.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Query();
        /// <summary>
        /// Insert data to repository;
        /// </summary>
        /// <param name="data"></param>
        void Insert(T data);
        /// <summary>
        /// Update data to repostiory;
        /// </summary>
        /// <param name="data"></param>
        void Update(T data);
        /// <summary>
        /// Delete data from repository;
        /// </summary>
        /// <param name="data"></param>
        void Delete(T data);
    }
}