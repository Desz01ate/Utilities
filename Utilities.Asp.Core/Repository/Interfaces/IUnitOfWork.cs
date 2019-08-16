using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Asp.Core.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbTransaction BeginTransaction();
        void SaveChanges(DbTransaction transaction);
        void RollbackChanges(DbTransaction transaction);
    }
}
