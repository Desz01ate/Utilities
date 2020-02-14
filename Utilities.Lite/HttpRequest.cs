using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
#if NETSTANDARD2_0
    /// <summary>
    /// Wrapper for standard Microsoft HttpClient request for GET,POST,PUT and DELETE
    /// </summary>
#else
    /// <summary>
    /// Wrapper for standard Microsoft HttpClient request for GET,POST,PUT,PATCH and DELETE
    /// </summary>
#endif
    public class HttpRequest : IDisposable
    {
        readonly Uri BaseUrl;
        readonly HttpClient Client;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUrl"></param>
        public HttpRequest(string baseUrl)
        {
            this.BaseUrl = new Uri(baseUrl);
            this.Client = new HttpClient();
        }
        public void AddDefaultHeaderValue(string key, string value, HttpRequest refClient)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            this.Client.DefaultRequestHeaders.Add(key, value);
        }
        public void ClearDefaultHeadersValue()
        {
            this.Client.DefaultRequestHeaders.Clear();
        }
        private bool disposed { get; set; }
        private void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                this.Client?.Dispose();
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Request GET to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, string Body)> GetAsync(string path)
        {
            var request = await Client.GetAsync(this.BaseUrl + path).ConfigureAwait(false);
            var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            return (request.StatusCode, response);
        }
        /// <summary>
        /// Request GET to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, T Body)> GetAsync<T>(string path)
        {
            var response = await GetAsync(path).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK) return (response.StatusCode, default);
            return (response.StatusCode, JsonConvert.DeserializeObject<T>(response.Body));
        }
        /// <summary>
        /// Request POST to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, string Body)> PostAsync(string path, object body)
        {
            HttpContent? requestBody = default;
            try
            {
                if (body is HttpContent content)
                {
                    requestBody = content;
                }
                else
                {
                    requestBody = DefaultContentBuilder(body);
                }
                var request = await Client.PostAsync(this.BaseUrl + path, requestBody).ConfigureAwait(false);
                var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                return (request.StatusCode, response);
            }
            finally
            {
                requestBody?.Dispose();
            }
        }
        /// <summary>
        /// Request POST to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, T Body)> PostAsync<T>(string path, object body)
        {
            var response = await PostAsync(path, body).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK) return (response.StatusCode, default);
            return (response.StatusCode, JsonConvert.DeserializeObject<T>(response.Body));
        }
        /// <summary>
        /// Request PUT to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, string Body)> PutAsync(string path, object body)
        {
            HttpContent? requestBody = default;
            try
            {
                if (body is HttpContent content)
                {
                    requestBody = content;
                }
                else
                {
                    requestBody = DefaultContentBuilder(body);
                }
                var request = await Client.PutAsync(this.BaseUrl + path, requestBody).ConfigureAwait(false);
                var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                return (request.StatusCode, response);
            }
            finally
            {
                requestBody?.Dispose();
            }
        }
        /// <summary>
        /// Request PUT to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, T Body)> PutAsync<T>(string path, object body)
        {
            var response = await PutAsync(path, body).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK) return (response.StatusCode, default);
            return (response.StatusCode, JsonConvert.DeserializeObject<T>(response.Body));
        }
#if NETSTANDARD2_1
        /// <summary>
        /// Request PATCH to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, string Body)> PatchAsync(string path, object body)
        {
            HttpContent? requestBody = default;
            try
            {
                if (body is HttpContent content)
                {
                    requestBody = content;
                }
                else
                {
                    requestBody = DefaultContentBuilder(body);
                }
                var request = await Client.PatchAsync(this.BaseUrl + path, requestBody).ConfigureAwait(false);
                var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                return (request.StatusCode, response);
            }
            finally
            {
                requestBody?.Dispose();
            }
        }
        /// <summary>
        /// Request PATCH to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, T Body)> PatchAsync<T>(string path, object body)
        {
            var response = await PatchAsync(path, body).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK) return (response.StatusCode, default);
            return (response.StatusCode, JsonConvert.DeserializeObject<T>(response.Body));
        }
#endif
        /// <summary>
        /// Request DELETE to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, string Body)> DeleteAsync(string path)
        {
            var request = await Client.DeleteAsync(this.BaseUrl + path).ConfigureAwait(false);
            var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            return (request.StatusCode, response);
        }
        /// <summary>
        /// Request DELETE to specific action of current host url and return status code with body.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode StatusCode, T Body)> DeleteAsync<T>(string path)
        {
            var response = await DeleteAsync(path).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK) return (response.StatusCode, default);
            return (response.StatusCode, JsonConvert.DeserializeObject<T>(response.Body));
        }
        /// <summary>
        /// Provide a way to build HttpContent from given body as a fallback when body is not HttpContent already.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected virtual HttpContent DefaultContentBuilder(object body)
        {
            return new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        }
    }
}