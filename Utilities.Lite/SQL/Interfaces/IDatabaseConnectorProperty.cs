using System.Collections.Generic;
using Utilities.Enum;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide an essential properties to database connector interface
    /// </summary>
    public interface IDatabaseConnectorProperty
    {
        string ConnectionString { get; }
        bool IsOpen { get; }
        Dictionary<SqlFunction, string> SQLFunctionConfiguration { get; }
    }
}