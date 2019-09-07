using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Utilities.Shared
{
    public static class DataTableExtension
    {
        /// <summary>
        /// Convert DataTable into IEnumerable of specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToList<T>(this DataTable data) where T : class, new()
        {
            using var dataReader = new DataTableReader(data);
            while (dataReader.Read())
            {
                yield return Data.RowBuilder<T>(dataReader);
            }
        }
        /// <summary>
        /// Convert DataTable into IEnumerable of dynamic.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToList(this DataTable data)
        {
            using var dataReader = new DataTableReader(data);
            var columns = dataReader.GetColumns();
            while (dataReader.Read())
            {
                yield return Data.RowBuilder(dataReader, columns);
            }
        }
    }
}
