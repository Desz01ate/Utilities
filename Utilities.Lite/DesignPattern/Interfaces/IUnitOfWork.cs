using System;
using System.Data;

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
        IDbTransaction BeginTransaction();
        /// <summary>
        /// Save changes made by given transaction.
        /// </summary>
        /// <param name="transaction"></param>

        void SaveChanges(IDbTransaction transaction);
        /// <summary>
        /// Rollback changes made by given transaction.
        /// </summary>
        /// <param name="transaction"></param>

        void RollbackChanges(IDbTransaction transaction);
    }
}