using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Utilities.Shared
{
    internal static class DataReaderExtension
    {
        /// <summary>
        /// Get underlying column name of the DbDataReader.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        internal static string[] GetColumns(this IDataReader dataReader)
        {
#if NET45
            if (dataReader == null) return new string[0];
#else
            if (dataReader == null) return Array.Empty<string>();
#endif
            return System.Linq.Enumerable.Range(0, dataReader.FieldCount).Select(dataReader.GetName).ToArray();
        }
    }
}