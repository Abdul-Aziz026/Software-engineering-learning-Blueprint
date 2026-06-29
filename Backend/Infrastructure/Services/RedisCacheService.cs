using System.Text.Json;
using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

/// <summary>
/// IDistributedCache-backed implementation of <see cref="ICacheService"/>.
/// Values are serialized to JSON (System.Text.Json) so any DTO round-trips
/// without coupling the Application layer to a serializer or to Redis itself.
/// Backed by Redis in production via AddStackExchangeRedisCache; falls back to
/// the in-memory distributed cache if no Redis connection is configured.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    // camelCase, ignore-null — keeps cached payloads small and stable.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes is null || bytes.Length == 0)
            return null;

        return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
        where T : class
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
        await _cache.SetAsync(key, bytes, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(key, cancellationToken);
}
