using System.Collections.Concurrent;

namespace Project_3.src.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // Key = IP + Endpoint
        private static readonly ConcurrentDictionary<string, (int Count, DateTime Window)> _requests = new();

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Login = 5 requests/min, everything else = 60 requests/min
            int limit = path == "/api/auth/login" ? 5 : 60;

            // Separate counter for each IP + endpoint
            var key = $"{ip}:{path}";

            var now = DateTime.UtcNow;

            if (_requests.TryGetValue(key, out var entry))
            {
                // Reset window every minute
                if ((now - entry.Window).TotalMinutes >= 1)
                {
                    entry = (1, now);
                }
                else
                {
                    entry.Count++;

                    if (entry.Count > limit)
                    {
                        var retryAfter = 60 - (int)(now - entry.Window).TotalSeconds;

                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        context.Response.Headers["Retry-After"] = retryAfter.ToString();

                        await context.Response.WriteAsync("Too Many Requests");
                        return;
                    }
                }

                _requests[key] = entry;
            }
            else
            {
                _requests[key] = (1, now);
            }

            await _next(context);
        }
    }
}

