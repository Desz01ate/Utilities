using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Utilities.Interfaces;

namespace Utilities.Shared
{
    public static class Data
    {
        /// <summary>
        /// Convert data row into POCO.
        /// </summary>
        /// <typeparam name="T">Class that implement IExcelReader</typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ConvertDataRowTo<T>(DataRow dr) where T : IExcelReader, new()
        {
            var properties = typeof(T).GetProperties();
            var obj = new T();

            for (var idx = 0; idx < properties.Length; idx++)
            {
                var property = properties[idx];
                var externalIndex = obj.GetExternalColumnIndex(property.Name);
                var value = dr[externalIndex];
                property.SetValue(obj, value);
            }
            return obj;
        }
    }
}
