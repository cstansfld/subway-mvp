using System.Globalization;
using MediatR;
using Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints.Filters;
using Subway.Mvp.Application.Features.FreshMenu.Meals;
using Subway.Mvp.Application.Features.FreshMenu.Meals.Get;
using Subway.Mvp.Application.Features.FreshMenu.Meals.GetAll;
using Subway.Mvp.Application.Features.FreshMenu.Meals.GetOne;
using Subway.Mvp.Application.Features.FreshMenu.Votes.Create;
using Subway.Mvp.Application.Features.FreshMenu.Votes.GetSummary;
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
        .AddEndpointFilter<FreshMenuDateAndMealFilter>()
        .WithTags("freshmenu v1").Produces<Result<MealOfTheDayDto>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");

        root.MapGet("meal", async (
            DayOfWeek dayOfTheWeek,
            ISender _sender,
            CancellationToken cancellationToken) =>
        {
            Result<MealByDayOfTheWeekResponse> result =
                await _sender.Send(new GetMealByDayOfTheWeekQuery(dayOfTheWeek, DateTime.UtcNow), cancellationToken);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result);
        })
        .MapToApiVersion(1)
        .AddEndpointFilter<FreshMenuDaysOfTheWeekFilter>()
        .WithTags("freshmenu v1").Produces<Result<MealByDayOfTheWeekResponse>>(200)
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

        root.MapPost("vote", async (
            string Meal,
            ISender _sender,
            CancellationToken cancellationToken) =>
        {
            Result<VoteForFreshMealResponse> result = await _sender.Send(
                new VoteForFreshMealCommand() { Meal = Meal }, cancellationToken);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result);
        })
        .MapToApiVersion(1)
        .AddEndpointFilter<FreshMenuMealFilter>()
        .WithTags("freshmenu vote summary v1").Produces<Result<VoteForFreshMealResponse>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");

        root.MapPost("votesummary", async (
            ISender _sender,
            CancellationToken cancellationToken) =>
                {
                    Result<AllVotesSummaryResponse> result = await _sender.Send(
                        new GetAllVotesQuery(), cancellationToken);
                    if (result.IsFailure)
                    {
                        return Results.BadRequest(result.Error);
                    }
                    return Results.Ok(result);
                })
        .MapToApiVersion(1)
        .WithTags("freshmenu vote summary v1").Produces<Result<AllVotesSummaryResponse>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");
    }
}
