using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utilities.Interfaces;

namespace Utilities
{
    public static class File
    {
        public static IEnumerable<T> ReadExcelAs<T>(string path, bool hasHeader = true, string table = null) where T : IExcelReader, new()
        {
            if (string.IsNullOrWhiteSpace(path) || !path.Contains(".xls"))
            {
                throw new FormatException("It seem that the path is not ending with .xls/.xlsx, please verify the path.");
            }
            var tableType = table ?? typeof(T).Name;
            List<T> dataset = new List<T>();
            using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var dt = result.Tables[tableType];
                    var skipBy = hasHeader ? 1 : 0;
                    foreach (DataRow row in dt.Rows.Cast<DataRow>().Skip(skipBy))
                    {
                        dataset.Add(Utilities.Shared.Data.ConvertDataRowTo<T>(row));
                    }
                }
            }
            return dataset;
        }
    }
}
