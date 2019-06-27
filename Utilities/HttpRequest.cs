using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    /// <summary>
    /// Wrapper for standard Microsoft HttpClient request for GET,POST,PUT and DELETE
    /// </summary>
    public static class HttpRequest
    {
        /// <summary>
        /// Send a GET request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static HttpResponseMessage Get(string url, Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var request = httpClient.GetAsync(url).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a GET request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static T GetFor<T>(string url, Dictionary<string, string> headers = null) where T : class, new()
        {
            var response = Get(url, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">Target endpoint</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var request = await httpClient.GetAsync(url);
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static async Task<T> GetAsyncFor<T>(string url, Dictionary<string, string> headers = null) where T : class, new()
        {
            var response = await GetAsync(url, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a POST request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
                var request = httpClient.PostAsync(url, bodyContent).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a POST request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static T PostFor<T>(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Post(url, body, encoding, mediatype, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
                var request = await httpClient.PostAsync(url, bodyContent);
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<T> PostAsyncFor<T>(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PostAsync(url, body, encoding, mediatype, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static HttpResponseMessage Put(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
                var request = httpClient.PutAsync(url, bodyContent).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a PUT request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static T PutFor<T>(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = Put(url, body, encoding, mediatype, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
                var request = await httpClient.PutAsync(url, bodyContent);
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<T> PutAsyncFor<T>(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PutAsync(url, body, encoding, mediatype, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        //public static HttpResponseMessage Patch(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        //{
        //    var httpClient = new HttpClient();
        //    var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
        //    var request = httpClient.PatchAsync(url, bodyContent).Result;
        //    return request;
        //}
        //public static async Task<HttpResponseMessage> PatchAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        //{
        //    var httpClient = new HttpClient();
        //    var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
        //    var request = await httpClient.PatchAsync(url, bodyContent);
        //    return request;
        //}
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(string url, Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var request = httpClient.DeleteAsync(url).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static T DeleteFor<T>(string url, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Delete(url, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var request = await httpClient.DeleteAsync(url);
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <returns></returns>
        public static async Task<T> DeleteAsyncFor<T>(string url, Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = await DeleteAsync(url, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
                var request = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = bodyContent
                }).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static T DeleteFor<T>(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Delete(url, body, encoding, mediatype, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
                var request = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = bodyContent
                });
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<T> DeleteAsyncFor<T>(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await DeleteAsync(url, body, encoding, mediatype, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a POST request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(body, encoding ?? Encoding.UTF8, mediatype);
                var request = httpClient.PostAsync(url, bodyContent).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a POST request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static T PostFor<T>(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Post(url, body, encoding, mediatype, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(body, encoding ?? Encoding.UTF8, mediatype);
                var request = await httpClient.PostAsync(url, bodyContent);
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<T> PostAsyncFor<T>(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PostAsync(url, body, encoding, mediatype, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static HttpResponseMessage Put(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(body, encoding ?? Encoding.UTF8, mediatype);
                var request = httpClient.PutAsync(url, bodyContent).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a PUT request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static T PutFor<T>(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Put(url, body, encoding, mediatype, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(body, encoding ?? Encoding.UTF8, mediatype);
                var request = await httpClient.PutAsync(url, bodyContent);
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<T> PutAsyncFor<T>(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PutAsync(url, body, encoding, mediatype, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(body, encoding ?? Encoding.UTF8, mediatype);
                var request = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = bodyContent
                }).Result;
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static T DeleteFor<T>(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Delete(url, body, encoding, mediatype, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var bodyContent = new StringContent(body, encoding ?? Encoding.UTF8, mediatype);
                var request = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = bodyContent
                });
                request.EnsureSuccessStatusCode();
                return request;
            }
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="mediatype">The body media type.</param>
        /// <returns></returns>
        public static async Task<T> DeleteAsyncFor<T>(string url, string body, Encoding encoding = null, string mediatype = "application/json", Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = await DeleteAsync(url, body, encoding, mediatype, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

    }
}
