using Application.Common.Interfaces.Services;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Llm;

/// <summary>
/// Embedding-side twin of <see cref="LlmFactory"/>: maps an <see cref="LlmProvider"/> to a cached
/// embedding generator (caching provided by <see cref="CachingProviderFactory{T}"/>).
/// Not every provider supports embeddings — Claude has no embeddings API and throws here.
/// </summary>
public class EmbeddingGeneratorFactory
    : CachingProviderFactory<IEmbeddingGenerator<string, Embedding<float>>>, IEmbeddingGeneratorFactory
{
    private readonly GeminiOptions _geminiOptions;
    private readonly ILoggerFactory _loggerFactory;

    public EmbeddingGeneratorFactory(
        IOptions<GeminiOptions> geminiOptions,
        ILoggerFactory loggerFactory)
    {
        _geminiOptions = geminiOptions.Value;
        _loggerFactory = loggerFactory;
    }

    public IEmbeddingGenerator<string, Embedding<float>> Create(LlmProvider provider) => GetOrCreate(provider);

    protected override IEmbeddingGenerator<string, Embedding<float>> Build(LlmProvider provider) => provider switch
    {
        LlmProvider.Gemini =>
            GeminiEmbeddingGenerator.CreateGeminiEmbeddingGenerator(_geminiOptions, _loggerFactory),

        // Anthropic has no embeddings API — fail loudly rather than return a wrong model.
        LlmProvider.Claude => throw new NotSupportedException(
            "The Claude provider has no embeddings API. Use Gemini for embeddings, or add a dedicated " +
            "embeddings provider (e.g. Voyage) as a new LlmProvider before requesting Claude embeddings."),

        _ => throw new NotSupportedException($"Embedding provider '{provider}' is not supported.")
    };
}
