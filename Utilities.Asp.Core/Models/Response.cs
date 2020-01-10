namespace Utilities.Asp.Core.Models
{
    /// <summary>
    /// Defined set of basic properties required for most HTTP response.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Indicated that the request has been successfully processed.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Optional message.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Optional data.
        /// </summary>
        public dynamic? Data { get; set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public Response(bool success, string message, dynamic data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        /// <summary>
        /// Ctor
        /// </summary>
        public Response()
        {
        }
    }
    /// <summary>
    /// Defined set of basic properties required for most HTTP response with generic data-object constraint.
    /// </summary>
    public class Response<T>
    {
        /// <summary>
        /// Indicated that the request has been successfully processed.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Optional message.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Optional data.
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public Response(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        /// <summary>
        /// Ctor
        /// </summary>
        public Response()
        {
        }
    }
}