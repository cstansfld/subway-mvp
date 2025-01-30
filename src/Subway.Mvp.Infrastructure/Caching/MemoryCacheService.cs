using System.Buffers;
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Subway.Mvp.Application.Abstractions.Caching;

namespace Subway.Mvp.Infrastructure.Caching;

public sealed class MemoryCacheService(IDistributedCache _cache) : ICacheService
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> CacheKeyLockManager = [];

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return _cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        T? value = await GetAsync<T>(key, cancellationToken);
        if (value is not null)
        {
            return value;
        }

        // only 1 thread enters the semaphore we wait for DefaultTimeSpan before returning true means thread entered the semaphore
        bool acquired = await GetOrCreateCacheLockByKey(key).WaitAsync(CacheOptions.CacheLockExpiration, cancellationToken);

        // if thread does not acquire the lock and timeout has expired
        if (!acquired)
        {
            return default;
        }

        try
        {
            // need to retry in case was updated
            value = await GetAsync<T>(key, cancellationToken);
            if (value is not null)
            {
                return value;
            }

            value = await factory(cancellationToken);

            if (value is null)
            {
                return default;
            }

            await SetAsync(key, value, expiration, cancellationToken);
        }
        finally
        {
            GetOrCreateCacheLockByKey(key).Release();
        }

        return value;
    }

    public async Task<bool> RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        // only 1 thread enters the semaphore we wait for DefaultTimeSpan before returning true means thread entered the semaphore
        bool acquired = await GetOrCreateCacheLockByKey(key).WaitAsync(CacheOptions.CacheLockExpiration, cancellationToken);

        // if thread does not acquire the lock and timeout has expired
        if (!acquired)
        {
            return false;
        }

        try
        {
            // need to retry in case was updated
            await _cache.RemoveAsync(key, cancellationToken);
        }
        finally
        {
            GetOrCreateCacheLockByKey(key).Release();
        }

        return true;
    }


    public Task Remove(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);

    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using var writer = new Utf8JsonWriter(buffer);

        JsonSerializer.Serialize(writer, value);

        return buffer.WrittenSpan.ToArray();
    }

    private static SemaphoreSlim GetOrCreateCacheLockByKey(string key)
    {
        return CacheKeyLockManager.GetOrAdd(key, new SemaphoreSlim(1, 1));
    }
}
