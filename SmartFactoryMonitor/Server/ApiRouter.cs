using SmartFactoryMonitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Server
{
    public class ApiRouter
    {
        private readonly Dictionary<string, Func<ApiRequest, Task<ApiResult>>> routes
            = new Dictionary<string, Func<ApiRequest, Task<ApiResult>>>(StringComparer.OrdinalIgnoreCase);

        public void Map(string method, string path, Func<ApiRequest, Task<ApiResult>> handler)
        {
            routes[Key(method, path)] = handler;
        }

        public bool TryResolve(string method, string path, out Func<ApiRequest, Task<ApiResult>> handler)
            => routes.TryGetValue(Key(method, path), out handler);

        private static string Key(string method, string path)
            => method.ToUpperInvariant() + " " + Normalize(path);

        private static string Normalize(string path)
            => string.IsNullOrWhiteSpace(path) ? "/" : (path.StartsWith("/") ? path : "/" + path);
    }
}