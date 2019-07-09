using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Provide an essential properties to database connector interface
    /// </summary>
    public interface IDatabaseConnectorProperty
    {
        string ConnectionString { get; }
        bool IsOpen { get; }
    }
}
