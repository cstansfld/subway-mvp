namespace Subway.Mvp.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);


    Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task Remove(string key, CancellationToken cancellationToken = default);
}
