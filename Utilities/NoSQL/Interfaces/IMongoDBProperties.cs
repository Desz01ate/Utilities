using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Enum;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide an essential properties to MongoDB connector interface
    /// </summary>
    public interface IMongoDBProperties
    {
        string ConnectionString { get; }
        bool IsOpen { get; }
        Dictionary<SqlFunction, string> SQLFunctionConfiguration { get; }
    }
}
