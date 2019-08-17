using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        void SaveChanges();
        void RollbackChanges();
    }
}
