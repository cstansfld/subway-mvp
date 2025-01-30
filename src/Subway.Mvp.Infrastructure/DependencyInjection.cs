using Microsoft.Extensions.DependencyInjection;
using Subway.Mvp.Application.Abstractions.Caching;
using Subway.Mvp.Infrastructure.Caching;

namespace Subway.Mvp.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(
       this IServiceCollection services)
    {
        return services.AddCache();
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache(options => options.SizeLimit = 2000 * 1024 * 1024);

        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }
}
