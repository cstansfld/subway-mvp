using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Application.Features.FreshMenu.Get;
using Subway.Mvp.Application.Features.FreshMenu.GetAll;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints;

internal static class FreshMenuEndpoints
{
    public static void MapFreshMenuEndpoints(this IEndpointRouteBuilder app)
    {
        WeatherReportEndpoint.MapWeatherReportEndpoint(app);

        RouteGroupBuilder root = app.MapGroup("freshmenu");

        root.MapGet("mealoftheday", async (
                    string? DateTimeUtc,
                    string? Meal,
                    ISender _sender,
                    CancellationToken cancellationToken) =>
        {
            DateTime dateTime = DateTimeUtc == default ? DateTime.Now : DateTime.Parse(DateTimeUtc, CultureInfo.InvariantCulture);
            Result<MealOfTheDayDto> result = await _sender.Send(new GetMealOfTheDayQuery(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), Meal), cancellationToken);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result);
        })
        .MapToApiVersion(1)
        .AddEndpointFilter<FreshMenuFilters>()
        .WithTags("freshmenu v1").Produces<Result<MealOfTheDayDto>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");

        root.MapGet("meal", async (
            DayOfWeek dayOfTheWeek,
            HybridCache _cache,
            IDocumentStore _documentStore,
            CancellationToken cancellationToken) =>
        {
            MealOfTheDay result = await _cache.GetOrCreateAsync(
                $"meals-by-day-{dayOfTheWeek}",
                async ct =>
                {
                    // Tuple here retuns info but testing caching and seeding
                    (string Day, MealOfTheDay _) = MealOfTheDay.GetDayInfo(dayOfTheWeek);
                    using IAsyncDocumentSession session = _documentStore.OpenAsyncSession();
                    MealOfTheDay meal = await
                        session.LoadAsync<MealOfTheDay>($"MealsOfTheDay/{Day}", ct);
                    return meal;
                },
                tags: [$"day-{dayOfTheWeek}", "meals"],
                cancellationToken: cancellationToken
            );
            if (result is null)
            {
                return Results.BadRequest($"No menu option for {dayOfTheWeek}");
            }
            return Results.Ok(result);
        })
        .MapToApiVersion(1)
        .WithTags("freshmenu v1").Produces<Result<Domain.FreshMenu.MealOfTheDay>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");

        root.MapGet("all", async (
            ISender _sender,
            CancellationToken cancellationToken) =>
        {
            Result<List<MealsOfTheDayResponse>> result = await _sender.Send(new GetMealsOfTheDayQuery(), cancellationToken);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result);
        })
        .MapToApiVersion(1)
        .WithTags("freshmenu v1").Produces<Result<List<MealsOfTheDayResponse>>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");
    }
}
