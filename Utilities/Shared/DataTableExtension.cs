using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utilities.SQL.Translator;

namespace Utilities.Shared
{
    /// <summary>
    /// Provide extensions for DataTable.
    /// </summary>
    public static class DataTableExtension
    {
        /// <summary>
        /// Convert DataTable into IEnumerable of specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this DataTable data) where T : class, new()
        {
            using var dataReader = new DataTableReader(data);
            var converter = new Converter<T>(dataReader);
            while (dataReader.Read())
            {
                yield return converter.CreateItemFromRow();
            }
        }

        /// <summary>
        /// Convert DataTable into IEnumerable of dynamic.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToEnumerable(this DataTable data)
        {
            using var dataReader = new DataTableReader(data);
            while (dataReader.Read())
            {
                yield return DataExtension.RowBuilder(dataReader);
            }
        }

        /// <summary>
        /// Get columns of DataTable.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetColumns(this DataTable data)
        {
            return System.Linq.Enumerable.Range(0, data.Columns.Count).Select(x => data.Columns[x].ColumnName);
        }
    }
}