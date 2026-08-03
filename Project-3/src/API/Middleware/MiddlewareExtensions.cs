namespace Project_3.src.API.Middleware
{
    public  static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimitingMiddleware>();
        }
        public static IApplicationBuilder UseRequestTiming(
       this IApplicationBuilder app) =>
        app.UseMiddleware<RequestTimingMiddleware>();
    }
}
