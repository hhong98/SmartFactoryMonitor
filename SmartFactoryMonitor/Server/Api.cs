using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Model
{
    public class ApiRequest
    {
        public string Method { get; }
        public string Path { get; }
        public NameValueCollection Query { get; }
        public NameValueCollection Headers { get; }
        public string Body { get; }

        public ApiRequest(string method, string path, NameValueCollection query, NameValueCollection headers, string body)
        {
            Method = method;
            Path = path;
            Query = query;
            Headers = headers;
            Body = body;
        }
    }

    public class ApiResult
    {
        public int StatusCode { get; }
        public Object Body { get; }
        public string ContentType { get; }

        public ApiResult(int statusCode, object body, string contentType = "application/json; charset=utf-8")
        {
            StatusCode = statusCode;
            Body = body;
            ContentType = contentType;
        }

        public static ApiResult Ok(object body) => new ApiResult(200, body);

        public static ApiResult Created(object body) => new ApiResult(201, body);

        public static ApiResult BadRequest(string message) => new ApiResult(400, new { ok = false, message });

        public static ApiResult NotFound(string path) => new ApiResult(404, new { ok = false, error = "not_found", path });

        public static ApiResult ServerError(string message) => new ApiResult(500, new { ok = false, error = "server_error", message });
    }
}