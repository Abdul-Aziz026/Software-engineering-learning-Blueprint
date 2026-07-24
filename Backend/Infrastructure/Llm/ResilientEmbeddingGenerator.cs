using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Llm;

/// <summary>
/// Embedding-side twin of <see cref="ResilientChatClient"/>: a <see cref="DelegatingEmbeddingGenerator{TInput,TEmbedding}"/>
/// (Decorator) that runs each provider call through <see cref="LlmRetryPolicy"/>. A single GenerateAsync
/// is the whole unit of work, so there's no tool-calling loop to worry about re-running.
/// </summary>
public sealed class ResilientEmbeddingGenerator
    : DelegatingEmbeddingGenerator<string, Embedding<float>>
{
    private static readonly TimeSpan AttemptTimeout = TimeSpan.FromSeconds(30);

    private readonly ILogger<ResilientEmbeddingGenerator> _logger;

    public ResilientEmbeddingGenerator(
        IEmbeddingGenerator<string, Embedding<float>> innerGenerator,
        ILogger<ResilientEmbeddingGenerator> logger)
        : base(innerGenerator)
    {
        _logger = logger;
    }

    public override Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default) =>
        LlmRetryPolicy.ExecuteAsync(
            ct => base.GenerateAsync(values, options, ct),
            AttemptTimeout,
            _logger,
            cancellationToken);
}
