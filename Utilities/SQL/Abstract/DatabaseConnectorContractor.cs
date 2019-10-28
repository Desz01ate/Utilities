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
        /// Connection string of this object.
        /// </summary>
        public string ConnectionString { get; protected set; }
        /// <summary>
        /// Determine whether the connection is open or not.
        /// </summary>
        public bool IsOpen { get; protected set; }
        /// <summary>
        /// SQL-function configuration for LINQ usage.
        /// </summary>
        internal protected Dictionary<SqlFunction, string> SQLFunctionConfiguration { get; private protected set; }
        /// <summary>
        /// Provide converter to convert data type from CLR to underlying SQL type, default mapper is supported by SQL Server and can be override when necessary.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal protected abstract string MapCLRTypeToSQLType(Type type);
    }
}
