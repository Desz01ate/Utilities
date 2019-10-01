//using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utilities.Interfaces;
using Utilities.Shared;

namespace Utilities
{
    /// <summary>
    /// Contains File I/O operation over Json,Csv,Xml file type.
    /// </summary>
    public static class File
    {
        /// <summary>
        /// Read json file and deserialize to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to target json file.</param>
        /// <param name="fileMode">A constant that determines how to open or create the file.</param>
        /// <returns></returns>
        public static T ReadJsonAs<T>(string path, FileMode fileMode = FileMode.Open) where T : new()
        {
            if (string.IsNullOrWhiteSpace(path) || !path.ToLower().EndsWith("json"))
            {
                throw new FormatException("It seem that the path is not ending with .json, please verify the path.");
            }
            using var fs = new FileStream(path, fileMode);
            using var sr = new StreamReader(fs);
            var content = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Read XML file and deserialize to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to target XML file.</param>
        /// <param name="fileMode">A constant that determines how to open or create the file.</param>
        /// <param name="omitRootObject">Omits writing the root object.</param>
        /// <returns></returns>
        public static T ReadXmlAs<T>(string path, FileMode fileMode = FileMode.Open, bool omitRootObject = true) where T : new()
        {
            if (string.IsNullOrWhiteSpace(path) || !path.ToLower().EndsWith("xml"))
            {
                throw new FormatException("It seem that the path is not ending with .xml, please verify the path.");
            }
            using var fs = new FileStream(path, fileMode);
            using var sr = new StreamReader(fs);
            var content = sr.ReadToEnd();
            var xmlDoc = new XmlDocument();
            var json = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.None, omitRootObject);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Read json file and deserialize to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to target json file.</param>
        /// <param name="fileMode">A constant that determines how to open or create the file.</param>
        /// <returns></returns>
        public static async Task<T> ReadJsonAsAsync<T>(string path, FileMode fileMode = FileMode.Open) where T : new()
        {
            if (string.IsNullOrWhiteSpace(path) || path.ToLower().EndsWith("json"))
            {
                throw new FormatException("It seem that the path is not ending with .json, please verify the path.");
            }
            using var fs = new FileStream(path, fileMode);
            using var sr = new StreamReader(fs);
            var content = await sr.ReadToEndAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Read xml file and deserialize to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to target xml file.</param>
        /// <param name="fileMode">A constant that determines how to open or create the file.</param>
        /// <param name="omitRootObject">Omits writing the root object.</param>
        /// <returns></returns>
        public static async Task<T> ReadXmlAsAsync<T>(string path, FileMode fileMode = FileMode.Open, bool omitRootObject = true) where T : new()
        {
            if (string.IsNullOrWhiteSpace(path) || path.ToLower().EndsWith("xml"))
            {
                throw new FormatException("It seem that the path is not ending with .xml, please verify the path.");
            }
            using var fs = new FileStream(path, fileMode);
            using var sr = new StreamReader(fs);
            var content = await sr.ReadToEndAsync().ConfigureAwait(false);
            var xmlDoc = new XmlDocument();
            var json = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.None, omitRootObject);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Read csv file and transform input into given CSV (you still need to manually give custom implement via ICSVReader)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReadCsvAs<T>(string path, bool hasHeader = false) where T : class, ICSVReader, new()
        {
            if (string.IsNullOrWhiteSpace(path) || path.ToLower().EndsWith("xml"))
            {
                throw new FormatException("It seem that the path is not ending with .csv, please verify the path.");
            }
            IEnumerable<string> content = System.IO.File.ReadAllLines(path);
            var skipBy = hasHeader ? 1 : 0;
            foreach (var line in content.Skip(skipBy))
            {
                var obj = new T();
                obj.ReadFromCSV(line);
                yield return obj;
            }
        }

        /// <summary>
        /// Serialize given object and write to json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <returns></returns>
        public static void WriteAsJson(object obj, string path, Encoding encoding)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object must not be null.");
            }
            if (string.IsNullOrWhiteSpace(path) || !path.ToLower().EndsWith("json"))
            {
                throw new FormatException("It seem that the path is not ending with .json, please verify the path.");
            }
            var json = JsonConvert.SerializeObject(obj);
            System.IO.File.WriteAllText(path, json, encoding);
        }

        /// <summary>
        /// Serialize given object and write to csv file.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <param name="fileMode">File mode.</param>
        public static void WriteAsCsv(IEnumerable<object> obj, string path, Encoding encoding, FileMode fileMode = FileMode.OpenOrCreate)
        {
            if (obj == null && obj.Count() == 0)
            {
                throw new ArgumentNullException("Object must not be null and must contains atleast one element.");
            }
            if (string.IsNullOrWhiteSpace(path) || !path.ToLower().EndsWith("csv"))
            {
                throw new FormatException("It seem that the path is not ending with .csv, please verify the path.");
            }

            var properties = obj.First().GetType().GetProperties();
            var content = new StringBuilder();
            content.AppendLine(string.Join(" ", properties.Select(x => x.Name)));
            foreach (var o in obj)
            {
                //micro-optimization via array and indexer
                var values = new string[properties.Count()];
                for (var idx = 0; idx < properties.Length; idx++)
                {
                    var value = properties[idx].GetValue(o).ToString();
                    values[idx] = value;
                }
                content.AppendLine(string.Join(" ", values));
            }
            System.IO.File.WriteAllText(path, content.ToString(), encoding);
        }

        /// <summary>
        /// Serialize given object and write to csv file.
        /// </summary>
        /// <param name="data">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <param name="fileMode">File mode.</param>
        public static void WriteAsCsv(object data, string path, Encoding encoding, FileMode fileMode = FileMode.OpenOrCreate)
        {
            WriteAsCsv(new[] { data }, path, encoding, fileMode);
        }
#if false
        /// <summary>
        /// Read excel file and convert to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to excel file.</param>
        /// <param name="hasHeader">Is the file has header.</param>
        /// <param name="sheetIndex">Index of target sheet in excel file.</param>
        /// <returns></returns>
        public static IEnumerable<T> ReadExcelAs<T>(string path, bool hasHeader = true, int sheetIndex = 0) where T : IExcelReader, new()
        {
            if (string.IsNullOrWhiteSpace(path) || !path.Contains(".xls"))
            {
                throw new FormatException("It seem that the path is not ending with .xls/.xlsx, please verify the path.");
            }
            List<T> dataset = new List<T>();
            using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var dt = result.Tables[sheetIndex];
                    var skipBy = hasHeader ? 1 : 0;
                    foreach (DataRow row in dt.Rows.Cast<DataRow>().Skip(skipBy))
                    {
                        dataset.Add(Utilities.Shared.Data.ConvertDataRowTo<T>(row));
                    }
                }
            }
            return dataset;
        }

        /// <summary>
        /// Read excel file and convert to data table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to excel file.</param>
        /// <param name="sheetIndex">Index of target sheet in excel file.</param>
        /// <param name="configuration">Dataset configuration, see details at https://github.com/ExcelDataReader/ExcelDataReader</param>
        /// <returns></returns>
        public static DataTable ReadExcelAsDataTable(string path, int sheetIndex = 0, ExcelDataSetConfiguration configuration = null)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.Contains(".xls"))
            {
                throw new FormatException("It seem that the path is not ending with .xls/.xlsx, please verify the path.");
            }
            using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(configuration);
                    var dt = result.Tables[sheetIndex];
                    return dt;
                }
            }
        }

        /// <summary>
        /// Read excel file and convert to dynamic object.
        /// </summary>
        /// <param name="path">Path to excel file.</param>
        /// <param name="hasHeader">Is the file has header.</param>
        /// <param name="sheetIndex">Index of target sheet in excel file.</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ReadExcel(string path, int sheetIndex = 0, bool hasHeader = true)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.Contains(".xls"))
            {
                throw new FormatException("It seem that the path is not ending with .xls/.xlsx, please verify the path.");
            }
            List<dynamic> dataset = new List<dynamic>();
            using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var dt = result.Tables[sheetIndex];
                    var skipBy = 0;
                    IEnumerable<string> columns = null;
                    if (hasHeader)
                    {
                        skipBy = 1;
                        columns = dt.Rows[0].ItemArray.Select(x => x.ToString().Replace(' ', '_'));
                    }
                    else
                    {
                        columns = dt.GetColumns();
                    }
                    foreach (DataRow row in dt.Rows.Cast<DataRow>().Skip(skipBy))
                    {
                        dataset.Add(Utilities.Shared.Data.RowBuilder(row, columns));
                    }
                }
            }
            return dataset;
        }
#endif
#if NETSTANDARD2_1
        /// <summary>
        /// Serialize given object and write to json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <param name="fileMode">File mode.</param>
        public static async Task WriteAsJsonAsync(IEnumerable<object> obj, string path, Encoding encoding, FileMode fileMode = FileMode.OpenOrCreate)
        {
            if (obj == null && obj.Count() == 0)
            {
                throw new ArgumentNullException("Object must not be null and must contains at least one element.");
            }
            if (string.IsNullOrWhiteSpace(path) || !path.ToLower().EndsWith("json"))
            {
                throw new FormatException("It seem that the path is not ending with .json, please verify the path.");
            }
            var json = JsonConvert.SerializeObject(obj);
            await System.IO.File.WriteAllTextAsync(path, json, encoding).ConfigureAwait(false);
        }
        /// <summary>
        /// Serialize given object and write to json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <param name="fileMode">File mode.</param>
        public static async Task WriteAsJsonAsync(object obj, string path, Encoding encoding, FileMode fileMode = FileMode.OpenOrCreate)
        {
            await WriteAsJsonAsync(new[] { obj }, path, encoding, fileMode).ConfigureAwait(false);
        }
#endif
    }
}