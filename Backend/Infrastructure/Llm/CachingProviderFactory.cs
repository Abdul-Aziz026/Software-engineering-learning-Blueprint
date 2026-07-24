using System.Collections.Concurrent;
using Domain.Enums;

namespace Infrastructure.Llm;

/// <summary>
/// Shared cache for the LLM provider factories (<see cref="LlmFactory"/> chat clients,
/// <see cref="EmbeddingGeneratorFactory"/> embedding generators). The factories are singletons, so
/// this caches ONE built pipeline (and its SDK-owned HttpClient) per provider for the app's lifetime
/// instead of reconstructing a client on every request. <see cref="Lazy{T}"/> guarantees a single
/// construction even under concurrent first calls.
/// </summary>
public abstract class CachingProviderFactory<T>
{
    private readonly ConcurrentDictionary<LlmProvider, Lazy<T>> _cache = new();

    protected T GetOrCreate(LlmProvider provider) =>
        _cache.GetOrAdd(provider, p => new Lazy<T>(() => Build(p))).Value;

    /// <summary>Builds the provider pipeline on first request; throws for unsupported providers.</summary>
    protected abstract T Build(LlmProvider provider);
}
