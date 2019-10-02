using System;
using System.Data;

namespace Utilities.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction BeginTransaction();

        void SaveChanges(IDbTransaction transaction);

        void RollbackChanges(IDbTransaction transaction);
    }
}