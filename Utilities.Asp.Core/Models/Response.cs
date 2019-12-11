namespace Utilities.Asp.Core.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        public Response(bool success, string message, dynamic data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public Response()
        {
        }
    }
}