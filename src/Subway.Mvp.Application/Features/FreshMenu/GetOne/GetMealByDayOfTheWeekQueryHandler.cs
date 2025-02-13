using Microsoft.Extensions.Caching.Hybrid;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Subway.Mvp.Application.Abstractions;
using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Features.FreshMenu.GetOne;

internal sealed class GetMealByDayOfTheWeekQueryHandler(HybridCache _cache,
            IDocumentStoreContainer _documentStore)
    : IQueryHandler<GetMealByDayOfTheWeekQuery, MealByDayOfTheWeekResponse>
{
    public async Task<Result<MealByDayOfTheWeekResponse>> Handle(GetMealByDayOfTheWeekQuery request, CancellationToken cancellationToken)
    {
        MealOfTheDay response = await _cache.GetOrCreateAsync(
            $"meals-by-day-{request.DayOfTheWeek}",
            async ct =>
            {
                // Tuple here returns info but testing caching and seeding
                (string Day, MealOfTheDay _) = MealOfTheDay.GetMealByDayInfo(request.DayOfTheWeek);
                using IAsyncDocumentSession session = _documentStore.Store.OpenAsyncSession();
                MealOfTheDay meal = await
                    session.LoadAsync<MealOfTheDay>($"MealsOfTheDay/{Day}", ct);
                return meal;
            },
            tags: [$"day-{request.DayOfTheWeek}", "meals"],
            cancellationToken: cancellationToken
        );

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
