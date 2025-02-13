using Microsoft.Extensions.Caching.Hybrid;
using Raven.Client.Documents.Session;
using Subway.Mvp.Application.Abstractions;
using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Features.FreshMenu.GetOne;

internal sealed class GetMealByDayOfTheWeekQueryHandler(HybridCache _cache,
            IDocumentStoreContainer _documentStore, ILogger<GetMealByDayOfTheWeekQueryHandler> _logger)
    : IQueryHandler<GetMealByDayOfTheWeekQuery, MealByDayOfTheWeekResponse>
{
    public async Task<Result<MealByDayOfTheWeekResponse>> Handle(GetMealByDayOfTheWeekQuery request, CancellationToken cancellationToken)
    {
        // logging injection is not available on preview cache
        bool cacheHit = true;
        MealOfTheDay response = await _cache.GetOrCreateAsync(
            $"meals-by-day-{request.DayOfTheWeek}", // ToDo DayOfWeek enum will output Thursday on string interpolation
            async ct =>
            {
                // Tuple here returns info but testing caching and seeding
                cacheHit = false;
                (string Day, MealOfTheDay _) = MealOfTheDay.GetMealByDayInfo(request.DayOfTheWeek);
                using IAsyncDocumentSession session = _documentStore.Store.OpenAsyncSession();
                MealOfTheDay meal = await
                    session.LoadAsync<MealOfTheDay>($"MealsOfTheDay/{Day}", ct);
                _logger.LogInformation("CacheMiss DB: {Database} - MealsOfTheDay/{Day}", _documentStore.Store.Database, Day);
                return meal;
            },
            tags: [$"day-{request.DayOfTheWeek}", "meals"],
            cancellationToken: cancellationToken
        );

        if (cacheHit)
        {
            _logger.LogInformation("CacheHit meals-by-day-{DayOfTheWeek}", request.DayOfTheWeek);
        }

        if (response is not null)
        {
            return MealByDayOfTheWeekResponse.Create(
                    MealOfTheDay.GetDayName(request.DayOfTheWeek),
                    response.Meal,
                    request.RequestDateTimeUtc,
                    DateTime.UtcNow);
        }

        return Result.Failure<MealByDayOfTheWeekResponse>(MealOfTheDayErrors.MealNotFoundError);

    }
}
