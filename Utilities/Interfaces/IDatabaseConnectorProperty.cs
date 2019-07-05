using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Interfaces
{
    interface IDatabaseConnectorProperty
    {
        string ConnectionString { get; }
        bool IsOpen { get; }
    }
}
