using System.Net;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Domain.FreshMenu;

namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints;

public class FreshMenuFilters : IEndpointFilter
{
    public virtual async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        string meal = context.GetArgument<string>(1);

        if (!string.IsNullOrWhiteSpace(meal) && !MealOfTheDayDto.GetAll().Any(x => x.Meal! == meal))
        {
            return Results.Problem(MealOfTheDayErrors.InvalidMealError.Code,
                $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}",
                (int)HttpStatusCode.BadRequest,
                MealOfTheDayErrors.InvalidMealError.Description);
        }
        return await next(context);
    }
}
