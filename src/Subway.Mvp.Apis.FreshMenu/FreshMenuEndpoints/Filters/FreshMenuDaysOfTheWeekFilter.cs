
using System.Net;
using Subway.Mvp.Domain.FreshMenu;

namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints.Filters;

public class FreshMenuDaysOfTheWeekFilter : IEndpointFilter
{
    private static readonly HashSet<int> ValidDaysOfTheWeek = [0, 1, 2, 3, 4, 5, 6];
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // invalid dayOfTheWeek
        int? dayOfTheWeek = context.GetArgument<int>(0);
        // not null and int is in ValidDaysOfTheWeek
        if (!dayOfTheWeek.HasValue || !ValidDaysOfTheWeek.TryGetValue(dayOfTheWeek.Value, out _))
        {
            return Results.Problem(MealOfTheDayErrors.InvalidDayOfTheWeekError.Code,
                $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}",
                (int)HttpStatusCode.BadRequest,
                MealOfTheDayErrors.InvalidDayOfTheWeekError.Description,
                type: $"{MealOfTheDayErrors.InvalidDayOfTheWeekError.Type}",
                extensions: new Dictionary<string, object?> { ["requestId"] = context.HttpContext.TraceIdentifier });
        }
        return await next(context);
    }
}
