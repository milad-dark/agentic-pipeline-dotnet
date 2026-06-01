using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace AgenticPipeline.Infrastructure.Cache;

public sealed class RedisCacheService(IDistributedCache cache)
{
    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var payload = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, payload, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var payload = await cache.GetStringAsync(key, ct);
        return payload is null ? default : JsonSerializer.Deserialize<T>(payload);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default) => cache.RemoveAsync(key, ct);
}
