using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Subway.Mvp.Application.Abstractions;
using Subway.Mvp.Application.Abstractions.Lifetime;
using Subway.Mvp.Domain.FreshMenu;

namespace Subway.Mvp.Infrastructure.Lifetime;

public sealed class FreshMenuDataStoreService(IServiceScopeFactory _serviceScopeFactory,
    ILogger<FreshMenuDataStoreService> _logger, HybridCache _cache,
    IApplicationLifetimeService _applicationLifetimeService) : IHostedService
#pragma warning restore CS9113 // Parameter is unread.
{
    private readonly string HostedSrvcName = nameof(FreshMenuDataStoreService);

    private bool DataRequiresSeeding
    {
        get;
        set;
    }

    private bool SeededDataCheckErrors
    {
        get;
        set;
    }

    private bool CacheLoaded
    {
        get;
        set;
    }

    private async Task InitializeMealsOfTheWeek(IDocumentStore documentStore, CancellationToken cancellationToken)
    {
        using IAsyncDocumentSession session = documentStore.OpenAsyncSession();
        await session.StoreAsync(MealOfTheDay.Monday, "MealsOfTheDay/Monday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Tuesday, "MealsOfTheDay/Tuesday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Wednesday, "MealsOfTheDay/Wednesday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Thursday, "MealsOfTheDay/Thursday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Friday, "MealsOfTheDay/Friday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Saturday, "MealsOfTheDay/Saturday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Sunday, "MealsOfTheDay/Sunday", cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }

    private async Task SetupMealsOfTheWeekInCache(IDocumentStore documentStore, CancellationToken cancellationToken)
    {
        using IAsyncDocumentSession session = documentStore.OpenAsyncSession();
        await GetOrCreateMealOfTheDay(DayOfWeek.Monday, session, cancellationToken);
        await GetOrCreateMealOfTheDay(DayOfWeek.Tuesday, session, cancellationToken);
        await GetOrCreateMealOfTheDay(DayOfWeek.Wednesday, session, cancellationToken);
        await GetOrCreateMealOfTheDay(DayOfWeek.Thursday, session, cancellationToken);
        await GetOrCreateMealOfTheDay(DayOfWeek.Friday, session, cancellationToken);
        await GetOrCreateMealOfTheDay(DayOfWeek.Saturday, session, cancellationToken);
        await GetOrCreateMealOfTheDay(DayOfWeek.Sunday, session, cancellationToken);
    }

    private async Task<MealOfTheDay> GetOrCreateMealOfTheDay(DayOfWeek dayOfTheWeek,
        IAsyncDocumentSession session, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"meals-by-day-{dayOfTheWeek}",
            async ct =>
            {
                (string Day, MealOfTheDay Meal) = MealOfTheDay.GetMealByDayInfo(dayOfTheWeek);
                Meal = await
                    session.LoadAsync<MealOfTheDay>($"MealsOfTheDay/{Day}", ct);
                return Meal;
            },
            tags: [$"day-{dayOfTheWeek}", "meals"],
            cancellationToken: cancellationToken
        );
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{HostedSrvcName} Starting", HostedSrvcName);
        _applicationLifetimeService.ServiceClosingEvent += ApplicationLifetimeService_ServiceClosingEvent;

        Task.Run(async () =>
        {
            while (!_applicationLifetimeService.ApplicationStopping && !CacheLoaded && !SeededDataCheckErrors)
            {
                try
                {
                    _logger.LogInformation("{HostedSrvcName} Time Event {AppTime}", HostedSrvcName, DateTimeOffset.UtcNow);

                    // if service is Singleton or Transient but if services are registered as Scoped
                    // for whatever reason (many) then this is safe way
                    await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
                    IDocumentStoreContainer documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStoreContainer>();
                    using IAsyncDocumentSession session = documentStore.Store.OpenAsyncSession();
                    if (await session.LoadAsync<MealOfTheDay>("MealsOfTheDay/Monday") == null)
                    {
                        // SeedsData
                        DataRequiresSeeding = true;
                        await InitializeMealsOfTheWeek(documentStore.Store, cancellationToken);
                        _logger.LogInformation("{HostedSrvcName} SeedingData {AppTime}", HostedSrvcName, DateTimeOffset.UtcNow);
                    }

                    // Fills the Cache
                    await SetupMealsOfTheWeekInCache(documentStore.Store, cancellationToken);
                    CacheLoaded = true;
                }
                catch (Exception ex)
                {
                    SeededDataCheckErrors = true;
                    _logger.LogError(ex, "{HostedSrvcName} Unhandled Exception Seeding Data {AppTime}", HostedSrvcName, DateTimeOffset.UtcNow);
                }
                await StopAsync(cancellationToken);
            }
        }, cancellationToken);

        _logger.LogInformation("{HostedSrvcName} Started", HostedSrvcName);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {

        _logger.LogInformation("{HostedSrvcName} SeededData {Seeded} CacheLoaded {Cache} Errors {SeedOrCacheErrors}",
            HostedSrvcName, DataRequiresSeeding, CacheLoaded, SeededDataCheckErrors);

        _logger.LogInformation("{HostedSrvcName} Stopping", HostedSrvcName);

        Task completedTask = Task.CompletedTask;

        _logger.LogInformation("{HostedSrvcName} Stopped", HostedSrvcName);

        return completedTask;
    }

    private void ApplicationLifetimeService_ServiceClosingEvent(object? sender, EventArgs e)
    {
        _logger.LogInformation("Received applicationClosing event.");

        _applicationLifetimeService.ServiceClosingEvent -= ApplicationLifetimeService_ServiceClosingEvent;
    }
}
