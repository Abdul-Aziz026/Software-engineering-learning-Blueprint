using System.Diagnostics;

namespace API.MiddleWare;

/// <summary>
/// Production request-logging middleware. For every HTTP request it:
///   1. Resolves a correlation id (reused from the inbound <c>X-Correlation-Id</c> header so a
///      gateway / upstream service can stitch logs across hops, otherwise freshly minted),
///   2. Opens an <see cref="ILogger"/> scope so <c>CorrelationId</c> (plus method &amp; path) ride
///      along on EVERY log line written while handling the request — yours and the framework's,
///   3. Times the request and emits a single structured "completed" log with the status code and
///      elapsed milliseconds, at a level chosen by the outcome (Info / Warn / Error).
///
/// The id is also stashed in <see cref="HttpContext.Items"/> (so downstream middleware such as the
/// exception handler can read it) and echoed on the response header (so a client can quote it).
/// </summary>
public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);

        context.Items[HeaderName] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // One scope -> these properties are attached to every log line for this request.
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestMethod"] = context.Request.Method,
            ["RequestPath"] = context.Request.Path.Value ?? string.Empty
        }))
        {
            var stopwatch = Stopwatch.GetTimestamp();
            try
            {
                await _next(context);
            }
            finally
            {
                var elapsedMs = Stopwatch.GetElapsedTime(stopwatch).TotalMilliseconds;
                var statusCode = context.Response.StatusCode;

                // Level reflects the outcome so dashboards/alerts can filter on it.
                var level = statusCode >= 500 ? LogLevel.Error
                          : statusCode >= 400 ? LogLevel.Warning
                          : LogLevel.Information;

                _logger.Log(level,
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs:0.0} ms",
                    context.Request.Method,
                    context.Request.Path.Value,
                    statusCode,
                    elapsedMs);
            }
        }
    }

    private static string ResolveCorrelationId(HttpContext context) =>
        context.Request.Headers.TryGetValue(HeaderName, out var incoming)
        && !string.IsNullOrWhiteSpace(incoming)
            ? incoming.ToString()
            : Guid.NewGuid().ToString("N");
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        => builder.UseMiddleware<CorrelationIdMiddleware>();
}
