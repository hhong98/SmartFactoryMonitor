using Newtonsoft.Json;
using NLog;
using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Server
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener listener;
        private CancellationTokenSource cts;
        private Logger logger;

        private readonly OracleService db;
        private readonly ApiRouter router;
        private readonly ApiController controller;

        /* 실제 안드로이드 폰 <-> WPF 서버
         * - 폰과 PC가 같은 WiFi에 연결
         * - WPF 서버는 localhost x, PC의 로컬 IP로 접근
         */

        private static readonly HashSet<string> allowedHost = new HashSet<string> {
                "http://127.0.0.1:8888/", // localhost
            };

        public HttpServer(OracleService dbService)
        {
            db = dbService;
            listener = new HttpListener();

            foreach (var host in allowedHost)
            {
                listener.Prefixes.Add(host);
            }

            logger = LogManager.GetLogger("SFM_Logger");

            router = new ApiRouter();
            controller = new ApiController(db);

            router.Map("GET", "/api/equipments/active", controller.ActiveEquipments);
            router.Map("GET", "/api/equipments/active/meta", controller.ActiveEquipmentsMeta);
            router.Map("GET", "/api/equipments/detail", controller.EquipmentDetail);
            router.Map("GET", "/api/simulator/equipments", controller.SimulatorEquipments);
            router.Map("POST", "/api/simulator/temperature", controller.SimulatorTemperature);
        }

        public void Start()
        {
            cts = new CancellationTokenSource();
            listener.Start();
            Task.Run(() => ListenLoop(cts.Token));
        }

        public void Stop()
        {
            cts.Cancel();
            listener.Stop();
        }

        public void Dispose()
        {
            Stop();
            listener.Close();
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!cts.IsCancellationRequested)
            {
                HttpListenerContext context = null;

                try
                {
                    context = await listener.GetContextAsync();
                    _ = Task.Run(() => RequestHandler(context), token);
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // 로그 남기기
                    logger.Error(ex.Message);
                }
            }
        }

        private async Task RequestHandler(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            try
            {
                InitCorsHeaders(res);

                if (req.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    res.StatusCode = (int)HttpStatusCode.NoContent; // 204
                    res.Close();
                    return;
                }

                string method = req.HttpMethod.ToUpperInvariant();
                string path = req.Url.AbsolutePath; // /api/equipments
                string body = await ReadBody(req);

                var apiReq = new ApiRequest(method, path, req.QueryString, req.Headers, body);

                if (!router.TryResolve(method, path, out var handler))
                {
                    await WriteJson(res, 404, ApiResult.NotFound(path).Body);
                    return;
                }

                var result = await handler(apiReq).ConfigureAwait(false);

                await WriteJson(res, result.StatusCode, result.Body);
            }
            catch (Exception ex)
            {
                await WriteJson(res, 500, new { error = "server_error", message = ex.Message });
            }
            finally
            {
                if (res.OutputStream.CanWrite)
                    res.Close();
            }
        }

        private void InitCorsHeaders(HttpListenerResponse res)
        {
            res.Headers["Access-Control-Allow-Origin"] = "*";
            res.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
            res.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
        }

        private static async Task WriteJson(HttpListenerResponse res, int statusCode, object data)
        {
            res.StatusCode = statusCode;
            res.ContentType = "application/json; charset=utf-8";

            var json = JsonConvert.SerializeObject(data);

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            res.ContentLength64 = buffer.Length;

            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task<string> ReadBody(HttpListenerRequest req)
        {
            if (!req.HasEntityBody) return null;

            using var reader = new StreamReader(
                req.InputStream,
                req.ContentEncoding ?? Encoding.UTF8);

            return await reader.ReadToEndAsync();
        }
    }
}