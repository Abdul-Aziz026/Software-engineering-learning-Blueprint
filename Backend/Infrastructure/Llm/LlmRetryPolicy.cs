using System.Net;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Llm;

/// <summary>
/// Shared transient-failure retry policy for the resilient LLM decorators (chat + embeddings):
/// a per-attempt timeout plus bounded retry with exponential backoff + jitter. Retries network
/// errors, 429s, and 5xx; never retries caller cancellation or client (4xx) errors.
/// </summary>
internal static class LlmRetryPolicy
{
    private const int MaxAttempts = 3;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Runs <paramref name="operation"/> under the retry policy. Each attempt gets a fresh token
    /// that fires on caller cancellation OR the per-attempt timeout.
    /// </summary>
    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        TimeSpan attemptTimeout,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(attemptTimeout);

            try
            {
                return await operation(timeoutCts.Token);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw; // Caller cancelled — never retry.
            }
            catch (Exception ex) when (attempt < MaxAttempts && IsTransient(ex, timeoutCts.IsCancellationRequested))
            {
                var delay = BaseDelay * Math.Pow(2, attempt - 1)
                            + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 250));

                logger.LogWarning(ex,
                    "Transient LLM failure on attempt {Attempt}/{MaxAttempts}; retrying in {DelayMs} ms",
                    attempt, MaxAttempts, (int)delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransient(Exception ex, bool timedOut) => ex switch
    {
        // Caller cancellation is filtered above, so an OCE here is our per-attempt timeout.
        OperationCanceledException => timedOut,

        HttpRequestException { StatusCode: HttpStatusCode.TooManyRequests } => true,          // 429
        HttpRequestException { StatusCode: >= HttpStatusCode.InternalServerError } => true,   // 5xx
        HttpRequestException { StatusCode: null } => true,                                    // DNS / socket / reset

        _ => false, // 4xx — retrying can't fix these
    };
}
