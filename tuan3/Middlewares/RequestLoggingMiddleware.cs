using System.Diagnostics;
using System.Net;

namespace tuan3.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {

            var watch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path;
            Console.WriteLine($"[log] bat dau xu ly : ");
            await _next(context);
            watch.Stop();
            var statusCode = context.Response.StatusCode;
            var duration = watch.ElapsedMilliseconds;
            Console.WriteLine($"[{method}] {path} => {statusCode} ({duration}ms)");
        }
    }
}
