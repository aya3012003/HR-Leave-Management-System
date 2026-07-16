using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.ExceptionHandling;
using System.ComponentModel.DataAnnotations;

namespace Project_3.src.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                ValidationException => (StatusCodes.Status422UnprocessableEntity, "Validation Error"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error")
            };

            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}