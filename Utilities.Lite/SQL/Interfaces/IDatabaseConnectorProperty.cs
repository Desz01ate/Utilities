using System.Collections.Generic;
using Utilities.Enum;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide an essential properties to database connector interface
    /// </summary>
    public interface IDatabaseConnectorProperty
    {
        /// <summary>
        /// Connection string of this object.
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// Determine whether the connection is open or not.
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// SQL-function configuration for LINQ usage.
        /// </summary>
        Dictionary<SqlFunction, string> SQLFunctionConfiguration { get; }
    }
}