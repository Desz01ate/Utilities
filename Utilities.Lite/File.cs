﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utilities.Interfaces;

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
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
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
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
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
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
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
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            using var fs = new FileStream(path, fileMode);
            using var sr = new StreamReader(fs);
            var content = await sr.ReadToEndAsync().ConfigureAwait(false);
            var xmlDoc = new XmlDocument();
            var json = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.None, omitRootObject);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Read csv file / content and transform input into given class (you still need to manually give custom implement via ICSVReader)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReadCsvAs<T>(string content, bool hasHeader = false) where T : class, ICsvReader, new()
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            IEnumerable<string> data = System.IO.File.Exists(content) ? ReadFile(content) : ReadRaw(content);
            var skipBy = hasHeader ? 1 : 0;
            foreach (var line in data.Skip(skipBy))
            {
                var obj = new T();
                obj.ReadFromCsv(line);
                yield return obj;
            }
        }
        /// <summary>
        /// Read csv file and transform input into given class (you still need to manually give custom implement via ICSVReader)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadFile(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }
        /// <summary>
        /// Read csv file and transform input into given class (you still need to manually give custom implement via ICSVReader)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static IEnumerable<string> ReadRaw(string content)
        {
            return System.Text.RegularExpressions.Regex.Split(content, "\r\n|\r|\n");
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
                throw new ArgumentNullException(nameof(obj));
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
        public static void WriteAsCsv<TSource>(IEnumerable<TSource> obj, string path, Encoding encoding, string separator = ",")
        {
            if (obj == null && obj.Count() == 0)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (string.IsNullOrWhiteSpace(path) || !path.ToLower().EndsWith("csv"))
            {
                throw new FormatException("It seem that the path is not ending with .csv, please verify the path.");
            }

            var properties = Utilities.Shared.GenericExtension.CompileGetter<TSource>();
            var content = new StringBuilder();
            content.AppendLine(string.Join(separator, properties.Select(x => x.Name)));
            foreach (var o in obj)
            {
                //micro-optimization via array and indexer
                var values = new string[properties.Count()];
                for (var idx = 0; idx < properties.Length; idx++)
                {
                    var value = properties[idx].GetValue(o)?.ToString();
                    values[idx] = value;
                }
                content.AppendLine(string.Join(separator, values));
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
        public static void WriteAsCsv<TSource>(TSource data, string path, Encoding encoding, string separator = ",")
        {
            WriteAsCsv<TSource>(new[] { data }, path, encoding, separator);
        }
#if NETSTANDARD2_1
        /// <summary>
        /// Serialize given object and write to json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <param name="fileMode">File mode.</param>
        public static async Task WriteAsJsonAsync(IEnumerable<object> obj, string path, Encoding encoding)
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
        public static async Task WriteAsJsonAsync(object obj, string path, Encoding encoding)
        {
            await WriteAsJsonAsync(new[] { obj }, path, encoding).ConfigureAwait(false);
        }
#endif
    }
}