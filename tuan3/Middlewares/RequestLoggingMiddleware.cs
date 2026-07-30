using System.Diagnostics;
using System.Net;

namespace tuan3.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger=logger;
        }

        public async Task InvokeAsync(HttpContext context ) {

            _logger.LogInformation(" [log] xu ly {a} {Path}", context.Request.Method, context.Request.Path);
            var watchStop = Stopwatch.StartNew();
            await _next(context);
            watchStop.Stop();
            _logger.LogInformation("da xu ly xong thoi gian  ghi nhan {watchStop} (ms) , {Statuscode}    " ,watchStop.ElapsedMilliseconds,context.Response.StatusCode);
        }
    }
}
