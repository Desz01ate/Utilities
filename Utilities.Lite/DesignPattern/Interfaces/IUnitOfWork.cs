using System;
using System.Data;
using System.Data.Common;

namespace Utilities.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction BeginTransaction();

        void SaveChanges(IDbTransaction transaction);

        void RollbackChanges(IDbTransaction transaction);
    }
}