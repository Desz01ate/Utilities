using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Structs
{
    public struct DbParameterStruct
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
    }
}
