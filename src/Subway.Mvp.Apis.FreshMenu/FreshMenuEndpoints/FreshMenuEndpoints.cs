using MediatR;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Application.Features.FreshMenu.Get;
using Subway.Mvp.Application.Features.FreshMenu.GetAll;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints;

internal static class FreshMenuEndpoints
{
    public static void MapFreshMenuEndpoints(IEndpointRouteBuilder app)
    {
        WeatherReportEndpoint.MapWeatherReportEndpoint(app);

        RouteGroupBuilder root = app.MapGroup("freshmenu");

        root.MapGet("today", async (
                    DateTime? DateTimeUtc,
                    string? Meal,
                    ISender _sender,
                    CancellationToken cancellationToken) =>
        {
            Result<MealOfTheDayDto> result = await _sender.Send(new GetMealOfTheDayQuery(DateTimeUtc ?? DateTime.UtcNow, Meal), cancellationToken);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result);
        })
        .AddEndpointFilter<FreshMenuFilters>()
        .WithTags("freshmenu").Produces<Result<MealOfTheDayDto>>(200)
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
        .CacheOutput("Expire60")
        .WithTags("freshmenu").Produces<Result<List<MealsOfTheDayResponse>>>(200)
        .Produces<Error>(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("FreshMenu Endpoint");
    }
}
