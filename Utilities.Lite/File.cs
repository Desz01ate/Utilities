using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utilities
{
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
        /// Read xml file and deserialize to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Path to target xml file.</param>
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
            var content = await sr.ReadToEndAsync();
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
            var content = await sr.ReadToEndAsync();
            var xmlDoc = new XmlDocument();
            var json = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.None, omitRootObject);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// Serialize given object and write to json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <returns></returns>
        public static void WriteAsJson<T>(T obj, string path, Encoding encoding)
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
#if NETSTANDARD2_1
        /// <summary>
        /// Serialize given object and write to json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="path">Path to target json file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <returns></returns>
        public static async Task WriteAsJsonAsync<T>(T obj, string path, Encoding encoding, FileMode fileMode = FileMode.OpenOrCreate)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object must not be null.");
            }
            var json = JsonConvert.SerializeObject(obj);
            await System.IO.File.WriteAllTextAsync(path, json, encoding);
        }
#endif
    }
}
