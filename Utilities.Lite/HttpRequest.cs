using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enumerables;
using static Utilities.Enumerables.InternalHttpContent;

namespace Utilities
{
    /// <summary>
    /// Wrapper for standard Microsoft HttpClient request for GET,POST,PUT and DELETE
    /// </summary>
    public static partial class HttpRequest
    {
        /// <summary>
        /// Send a GET request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="headers"></param>
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
        /// <param name="headers"></param>
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
        /// <param name="headers"></param>
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
        /// <param name="headers"></param>
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
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
                HttpContent bodyContent = null;
                var innerContentType = httpContentTypeTranslator[contentType];
                switch (contentType)
                {
                    case HttpContentType.application_json:
                        bodyContent = new StringContent(body is string ? body : JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, innerContentType);
                        break;
                    case HttpContentType.application_x_www_form_urlencoded:
                        bodyContent = new FormUrlEncodedContent(body);
                        break;
                    case HttpContentType.multipart_formdata:
                    default:
                        throw new NotSupportedException($"{innerContentType} is currently not supported.");
                        bodyContent = new MultipartFormDataContent();
                        break;
                }
                var request = httpClient.PostAsync(url, bodyContent).Result;
                bodyContent.Dispose();
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static T PostFor<T>(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Post(url, body, encoding, contentType, headers);
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
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
                HttpContent bodyContent = null;
                var innerContentType = httpContentTypeTranslator[contentType];
                switch (contentType)
                {
                    case HttpContentType.application_json:
                        bodyContent = new StringContent(body is string ? body : JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, innerContentType);
                        break;
                    case HttpContentType.application_x_www_form_urlencoded:
                        bodyContent = new FormUrlEncodedContent(body);
                        break;
                    case HttpContentType.multipart_formdata:
                    default:
                        throw new NotSupportedException($"{innerContentType} is currently not supported.");
                        bodyContent = new MultipartFormDataContent();
                        break;
                }
                var request = await httpClient.PostAsync(url, bodyContent);
                bodyContent.Dispose();
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> PostAsyncFor<T>(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PostAsync(url, body, encoding, contentType, headers);
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
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
                HttpContent bodyContent = null;
                var innerContentType = httpContentTypeTranslator[contentType];
                switch (contentType)
                {
                    case HttpContentType.application_json:
                        bodyContent = new StringContent(body is string ? body : JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, innerContentType);
                        break;
                    case HttpContentType.application_x_www_form_urlencoded:
                        bodyContent = new FormUrlEncodedContent(body);
                        break;
                    case HttpContentType.multipart_formdata:
                    default:
                        throw new NotSupportedException($"{innerContentType} is currently not supported.");
                        bodyContent = new MultipartFormDataContent();
                        break;
                }
                var request = httpClient.PutAsync(url, bodyContent).Result;
                bodyContent.Dispose();
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static T PutFor<T>(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = Put(url, body, encoding, contentType, headers);
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
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
                HttpContent bodyContent = null;
                var innerContentType = httpContentTypeTranslator[contentType];
                switch (contentType)
                {
                    case HttpContentType.application_json:
                        bodyContent = new StringContent(body is string ? body : JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, innerContentType);
                        break;
                    case HttpContentType.application_x_www_form_urlencoded:
                        bodyContent = new FormUrlEncodedContent(body);
                        break;
                    case HttpContentType.multipart_formdata:
                    default:
                        throw new NotSupportedException($"{innerContentType} is currently not supported.");
                        bodyContent = new MultipartFormDataContent();
                        break;
                }
                var request = await httpClient.PutAsync(url, bodyContent);
                bodyContent.Dispose();
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> PutAsyncFor<T>(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PutAsync(url, body, encoding, contentType, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        //public static HttpResponseMessage Patch(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json)
        //{
        //    var httpClient = new HttpClient();
        //    var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, contentType);
        //    var request = httpClient.PatchAsync(url, bodyContent).Result;
        //    return request;
        //}
        //public static async Task<HttpResponseMessage> PatchAsync(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json)
        //{
        //    var httpClient = new HttpClient();
        //    var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, contentType);
        //    var request = await httpClient.PatchAsync(url, bodyContent);
        //    return request;
        //}
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="headers"></param>
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
        /// <param name="headers"></param>
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
        /// <param name="headers"></param>
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
        /// <param name="headers"></param>
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
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
                HttpContent bodyContent = null;
                var innerContentType = httpContentTypeTranslator[contentType];
                switch (contentType)
                {
                    case HttpContentType.application_json:
                        bodyContent = new StringContent(body is string ? body : JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, innerContentType);
                        break;
                    case HttpContentType.application_x_www_form_urlencoded:
                        bodyContent = new FormUrlEncodedContent(body);
                        break;
                    case HttpContentType.multipart_formdata:
                    default:
                        throw new NotSupportedException($"{innerContentType} is currently not supported.");
                        bodyContent = new MultipartFormDataContent();
                        break;
                }
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = bodyContent
                };
                var request = httpClient.SendAsync(message).Result;
                message.Dispose();
                bodyContent.Dispose();
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static T DeleteFor<T>(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Delete(url, body, encoding, contentType, headers);
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null)
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
                HttpContent bodyContent = null;
                var innerContentType = httpContentTypeTranslator[contentType];
                switch (contentType)
                {
                    case HttpContentType.application_json:
                        bodyContent = new StringContent(body is string ? body : JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, innerContentType);
                        break;
                    case HttpContentType.application_x_www_form_urlencoded:
                        bodyContent = new FormUrlEncodedContent(body);
                        break;
                    case HttpContentType.multipart_formdata:
                    default:
                        throw new NotSupportedException($"{innerContentType} is currently not supported.");
                        bodyContent = new MultipartFormDataContent();
                        break;
                }
                var message = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = bodyContent
                };
                var request = await httpClient.SendAsync(message);
                message.Dispose();
                bodyContent.Dispose();
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
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> DeleteAsyncFor<T>(string url, dynamic body, Encoding encoding = null, HttpContentType contentType = HttpContentType.application_json, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await DeleteAsync(url, body, encoding, contentType, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
    }
    /// <summary>
    /// Wrapper for standard Microsoft HttpClient request for GET,POST,PUT and DELETE
    /// </summary>
    public static partial class HttpRequest
    {
        /// <summary>
        /// Send a POST request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            var request = httpClient.PostAsync(url, body).Result;
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        /// <summary>
        /// Send a POST request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static T PostFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Post(url, body, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, HttpContent body, Dictionary<string, string> headers = null)
        {

            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            var request = await httpClient.PostAsync(url, body);
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="encoding">The body content type.</param>
        /// <param name="contentType">The body media type.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> PostAsyncFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PostAsync(url, body, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            var request = httpClient.PutAsync(url, body).Result;
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static T PutFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = Put(url, body, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            var request = await httpClient.PutAsync(url, body);
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> PutAsyncFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await PutAsync(url, body, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = body
            };
            var request = httpClient.SendAsync(message).Result;
            message.Dispose();
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static T DeleteFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = Delete(url, body, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            var message = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = body
            };
            var request = await httpClient.SendAsync(message);
            message.Dispose();
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="body">The body content the request is sent to.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> DeleteAsyncFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : class, new()
        {
            HttpResponseMessage response = await DeleteAsync(url, body, headers);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

#if NETSTANDARD2_1
        public static HttpResponseMessage Patch(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            return PatchAsync(url, body, headers).Result;
        }
        public static T PatchFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : new()
        {
            return PatchAsyncFor<T>(url, body, headers).Result;
        }

        public static async Task<HttpResponseMessage> PatchAsync(string url, HttpContent body, Dictionary<string, string> headers = null)
        {
            using var httpClient = new HttpClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            var request = await httpClient.PatchAsync(url, body);
            body.Dispose();
            request.EnsureSuccessStatusCode();
            return request;
        }
        public static async Task<T> PatchAsyncFor<T>(string url, HttpContent body, Dictionary<string, string> headers = null) where T : new()
        {
            var response = await PatchAsync(url, body, headers);
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
#endif
    }

}
