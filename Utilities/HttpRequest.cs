using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class HttpRequest
    {
        public static HttpResponseMessage Get(string url)
        {
            var httpClient = new HttpClient();
            var request = httpClient.GetAsync(url).Result;
            return request;
        }
        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            var httpClient = new HttpClient();
            var request = await httpClient.GetAsync(url);
            return request;
        }
        public static HttpResponseMessage Post(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        {
            var httpClient = new HttpClient();
            var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
            var request = httpClient.PostAsync(url, bodyContent).Result;
            return request;
        }
        public static async Task<HttpResponseMessage> PostAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        {
            var httpClient = new HttpClient();
            var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
            var request = await httpClient.PostAsync(url, bodyContent);
            return request;
        }
        public static HttpResponseMessage Put(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        {
            var httpClient = new HttpClient();
            var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
            var request = httpClient.PutAsync(url, bodyContent).Result;
            return request;
        }
        public static async Task<HttpResponseMessage> PutAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        {
            var httpClient = new HttpClient();
            var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
            var request = await httpClient.PutAsync(url, bodyContent);
            return request;
        }
        //currently broken as .NET Standard 2.0 and below doesn't contain the definition for the Patch.
        //public static HttpResponseMessage Patch(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        //{
        //    var httpClient = new HttpClient();
        //    var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
        //    var request = httpClient.PatchAsync(url, body).Result;
        //    return request;
        //}
        //public static async Task<HttpResponseMessage> PatchAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        //{
        //    var httpClient = new HttpClient();
        //    var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
        //    var request = await httpClient.PatchAsync(url, body);
        //    return request;
        //}
        public static HttpResponseMessage Delete(string url)
        {
            var httpClient = new HttpClient();
            var request = httpClient.DeleteAsync(url).Result;
            return request;
        }
        public static async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var httpClient = new HttpClient();
            var request = await httpClient.DeleteAsync(url);
            return request;
        }
        public static HttpResponseMessage Delete(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        {
            var httpClient = new HttpClient();
            var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
            var request = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = bodyContent
            }).Result;
            return request;
        }
        public static async Task<HttpResponseMessage> DeleteAsync(string url, dynamic body, Encoding encoding = null, string mediatype = "application/json")
        {
            var httpClient = new HttpClient();
            var bodyContent = new StringContent(JsonConvert.SerializeObject(body), encoding ?? Encoding.UTF8, mediatype);
            var request = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = bodyContent
            });
            return request;
        }
    }
}
