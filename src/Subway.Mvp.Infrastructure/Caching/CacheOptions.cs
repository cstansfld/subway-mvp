using Microsoft.Extensions.Caching.Distributed;

namespace Subway.Mvp.Infrastructure.Caching;
public static class CacheOptions
{
    public static int CacheLockExpiration => 1000;

    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null ?
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } :
            DefaultExpiration;
}
