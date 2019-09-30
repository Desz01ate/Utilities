using System;
using System.Data.Common;

namespace Utilities.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbTransaction BeginTransaction();

        void SaveChanges(DbTransaction transaction);

        void RollbackChanges(DbTransaction transaction);
    }
}