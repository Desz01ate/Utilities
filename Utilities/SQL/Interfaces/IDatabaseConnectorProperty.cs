using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Enumerables;

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
