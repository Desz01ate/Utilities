using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Utilities.Shared
{
    internal static class DataReaderExtension
    {
        /// <summary>
        /// Get underlying column name of the IDataReader.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        internal static string[] GetColumns(this IDataReader dataReader)
        {
            if (dataReader == null) return Array.Empty<string>();
            return Enumerable.Range(0, dataReader.FieldCount).Select(dataReader.GetName).ToArray();
        }
    }
}