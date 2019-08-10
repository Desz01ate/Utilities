using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Delegates
{
    public delegate Func<DbDataReader, T> DataBuilder<T>(DbDataReader dataReader) where T : new();
}
