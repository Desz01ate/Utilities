using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Enum;

namespace Utilities.SQL.Abstract
{
    /// <summary>
    /// Contractor class with properties required for internal and derived class.
    /// </summary>
    public abstract class DatabaseConnectorContractor
    {
        /// <summary>
        /// SQL-function configuration for LINQ usage.
        /// </summary>
        protected internal Dictionary<SqlFunction, string> SQLFunctionConfiguration { get; private protected set; }
        /// <summary>
        /// Provide converter to convert data type from CLR to underlying SQL type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected internal abstract string MapCLRTypeToSQLType(Type type);
    }
}
