using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Utilities.Interfaces;

namespace Utilities.DesignPattern.UnitOfWork.Components
{
    /// <summary>
    /// Repository class with IEnumerable functionality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDatabase"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class EnumeratableRepository<T, TDatabase, TParameter> : Repository<T, TDatabase, TParameter>, IEnumerable<T>
        where T : class, new()
        where TDatabase : DbConnection, new()
        where TParameter : DbParameter, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseConnector"></param>
        public EnumeratableRepository(IDatabaseConnectorExtension<TDatabase, TParameter> databaseConnector) : base(databaseConnector)
        {

        }
        /// <summary>
        /// Get enumerator of data repository.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            foreach (var data in Select())
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
