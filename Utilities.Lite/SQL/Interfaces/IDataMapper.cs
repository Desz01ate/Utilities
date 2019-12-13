using System.Collections.Generic;

namespace Utilities.Interfaces
{
    internal interface IDataMapper<out T> where T : new()
    {
        T GenerateObject();
        IEnumerable<T> GenerateObjects();
    }
}
