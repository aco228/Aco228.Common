using Microsoft.Extensions.Caching.Memory;

namespace Aco228.Common.Models;

/// <summary>
/// Typed in-memory cache with per-entry time to live.
/// Expired entries are never returned; they are removed on access and by a periodic
/// scan (at most once per <c>scanFrequency</c>) triggered by cache operations.
/// </summary>
public class TimedCacheManager<TObj> : IDisposable
{
    private readonly MemoryCache _cache;
    private readonly TimeSpan _defaultTtl;

    public TimedCacheManager(TimeSpan? defaultTtl = null, TimeSpan? scanFrequency = null)
    {
        _defaultTtl = defaultTtl ?? TimeSpan.FromMinutes(5);
        _cache = new MemoryCache(new MemoryCacheOptions
        {
            ExpirationScanFrequency = scanFrequency ?? TimeSpan.FromMinutes(1),
        });
    }

    public int Count => _cache.Count;

    public bool TryGet(string key, out TObj value)
    {
        if (_cache.TryGetValue(key, out TObj? cached))
        {
            value = cached!;
            return true;
        }

        value = default!;
        return false;
    }

    public TObj? Get(string key)
        => TryGet(key, out var value) ? value : default;

    public TObj AddOrUpdate(string key, TObj value, TimeSpan? ttl = null)
        => _cache.Set(key, value, ttl ?? _defaultTtl);

    public TObj AddOrUpdate(string key, TObj value, DateTimeOffset validUntil)
        => _cache.Set(key, value, validUntil);

    public TObj GetOrAdd(string key, Func<string, TObj> factory, TimeSpan? ttl = null)
    {
        if (TryGet(key, out var value))
            return value;

        return AddOrUpdate(key, factory(key), ttl);
    }

    public async Task<TObj> GetOrAddAsync(string key, Func<string, Task<TObj>> factory, TimeSpan? ttl = null)
    {
        if (TryGet(key, out var value))
            return value;

        return AddOrUpdate(key, await factory(key), ttl);
    }

    public void Remove(string key)
        => _cache.Remove(key);

    /// <summary>Removes all expired entries now, without waiting for the periodic scan.</summary>
    public void RemoveExpired()
        => _cache.Compact(0);

    public void Clear()
        => _cache.Clear();

    public void Dispose()
    {
        _cache.Dispose();
        GC.SuppressFinalize(this);
    }
}
