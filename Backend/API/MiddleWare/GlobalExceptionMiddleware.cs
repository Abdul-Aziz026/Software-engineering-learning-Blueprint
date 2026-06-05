using System.Text.Json;
using Domain.Exceptions;

namespace API.MiddleWare;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException ve     => (StatusCodes.Status400BadRequest,  ve.Message),
            AuthenticationException ae => (StatusCodes.Status401Unauthorized, ae.Message),
            NotFoundException ne       => (StatusCodes.Status404NotFound,    ne.Message),
            UnknownException ue        => (StatusCodes.Status500InternalServerError, ue.Message),
            _                          => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception at {Path}", context.Request.Path);
        else
            _logger.LogWarning(exception, "Handled domain exception at {Path}: {Message}", context.Request.Path, exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = new { statusCode, message };
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
