﻿using System.Globalization;
using System.Net;
using Subway.Mvp.Application.Features.FreshMenu.Meals;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Domain.FreshMenuVotes;

namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints.Filters;

public class FreshMenuDateAndMealFilter : IEndpointFilter
{
    public virtual async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // invalid dateformat
        string? date = context.GetArgument<string>(0);
        // not null and date has the dd component whch is not picked up py the tryParse (s)
        if (!string.IsNullOrWhiteSpace(date) && date.Split("-", StringSplitOptions.RemoveEmptyEntries).Length < 3 ||
            !string.IsNullOrWhiteSpace(date) && !DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            return Results.Problem(MealOfTheDayErrors.InvalidDateError.Code,
                $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}",
                (int)HttpStatusCode.BadRequest,
                MealOfTheDayErrors.InvalidDateError.Description,
                type: $"{MealOfTheDayErrors.InvalidDateError.Type}",
                extensions: new Dictionary<string, object?> { ["requestId"] = context.HttpContext.TraceIdentifier });
        }

        // invalid meal
        string meal = context.GetArgument<string>(1);
        if (!string.IsNullOrWhiteSpace(meal) && !MealOfTheDayDto.GetAll().Any(x => x.Meal!.Equals(meal, StringComparison.OrdinalIgnoreCase)))
        {
            return Results.Problem(MealOfTheDayErrors.InvalidMealError.Code,
                $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}",
                (int)HttpStatusCode.BadRequest,
                MealOfTheDayErrors.InvalidMealError.Description,
                type: $"{MealOfTheDayErrors.InvalidMealError.Type}",
                extensions: new Dictionary<string, object?> { ["requestId"] = context.HttpContext.TraceIdentifier });
        }
        return await next(context);
    }
}
