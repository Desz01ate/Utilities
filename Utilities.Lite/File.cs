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
