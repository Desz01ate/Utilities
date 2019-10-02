﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        public static IEnumerable<T> ToEnumerable<T>(this DataTable data, Func<DataTableReader, T> builder = null) where T : class, new()
        {
            using var dataReader = new DataTableReader(data);
            var action = builder ?? Data.RowBuilder<T>;
            while (dataReader.Read())
            {
                yield return action(dataReader);
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
            var columns = dataReader.GetColumns();
            while (dataReader.Read())
            {
                yield return Data.RowBuilder(dataReader, columns);
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