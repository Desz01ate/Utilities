using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.SQL.Interfaces
{
    internal interface IDataMapper<T> where T : new()
    {
        T GenerateObject();
        IEnumerable<T> GenerateObjects();
    }
}
