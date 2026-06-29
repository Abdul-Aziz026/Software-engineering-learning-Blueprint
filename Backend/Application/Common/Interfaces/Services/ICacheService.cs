namespace Application.Common.Interfaces.Services;

/// <summary>
/// A thin caching abstraction. The Application layer talks to this interface only —
/// it never sees Redis / IDistributedCache / StackExchange.Redis types. Swap the
/// implementation (in-memory, Redis, etc.) without touching any handler.
/// </summary>
public interface ICacheService
{
    /// <summary>Returns the cached value for <paramref name="key"/>, or null on a miss.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Stores <paramref name="value"/> under <paramref name="key"/> with a time-to-live.</summary>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Evicts a key. Called by write handlers to invalidate stale entries.</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
