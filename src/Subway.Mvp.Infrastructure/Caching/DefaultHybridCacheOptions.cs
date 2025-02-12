

using Microsoft.Extensions.Caching.Hybrid;

namespace Subway.Mvp.Infrastructure.Caching;

public static class DefaultHybridCacheOptions
{
    public static int CacheLockExpiration => 1000;

    public static HybridCacheEntryOptions DefaultCacheSettings => new()
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(30)
    };

    public static HybridCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null ?
            new HybridCacheEntryOptions { Expiration = expiration, 
                LocalCacheExpiration = DefaultCacheSettings.LocalCacheExpiration } :
            DefaultCacheSettings;
}
