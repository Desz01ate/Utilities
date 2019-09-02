﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Utilities.Shared
{
    internal static class DataReaderExtension
    {
        /// <summary>
        /// Get underlying column name of the DbDataReader.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetColumns(this DbDataReader dataReader)
        {
            if (dataReader == null) throw new ArgumentNullException($"{nameof(dataReader)} must not be null.");
            return System.Linq.Enumerable.Range(0, dataReader.FieldCount).Select(dataReader.GetName);
        }
    }
}
