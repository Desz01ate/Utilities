using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Structs
{
    /// <summary>
    /// Provide an abstract layer for IDbParameter for using in a non-generic environment.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct DbParameterStruct
    {
        /// <summary>
        /// Name of parameter;
        /// </summary>
        public readonly string ParameterName;
        /// <summary>
        /// Value of parameter;
        /// </summary>
        public readonly object ParameterValue;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="parameterValue">Value of parameter</param>
        public DbParameterStruct(string parameterName, object parameterValue)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue ?? DBNull.Value;
        }
    }
}
