using System;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Unit of work interface.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Start database transaction.
        /// </summary>
        /// <returns></returns>
        void BeginTransaction();
        /// <summary>
        /// Save changes made by given transaction.
        /// </summary>

        void SaveChanges();
        /// <summary>
        /// Rollback changes made by given transaction.
        /// </summary>
        void RollbackChanges();
    }
}