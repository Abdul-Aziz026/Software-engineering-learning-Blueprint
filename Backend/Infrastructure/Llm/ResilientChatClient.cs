using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Llm;

/// <summary>
/// Wraps a provider IChatClient with <see cref="LlmRetryPolicy"/> (per-attempt timeout + bounded
/// retry for transient failures).
///
/// Placement matters: this sits INSIDE FunctionInvokingChatClient in the pipeline, so each individual
/// provider round-trip is retried — not the whole multi-step tool-calling loop (which would re-execute
/// tools). Provider SDKs that throw their own exception types get added to <see cref="LlmRetryPolicy"/>
/// as they are observed in production logs.
/// </summary>
public sealed class ResilientChatClient : DelegatingChatClient
{
    private static readonly TimeSpan AttemptTimeout = TimeSpan.FromSeconds(100);

    private readonly ILogger<ResilientChatClient> _logger;

    public ResilientChatClient(IChatClient innerClient, ILogger<ResilientChatClient> logger)
        : base(innerClient)
    {
        _logger = logger;
    }

    public override Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default) =>
        LlmRetryPolicy.ExecuteAsync(
            ct => base.GetResponseAsync(messages, options, ct),
            AttemptTimeout,
            _logger,
            cancellationToken);
}
