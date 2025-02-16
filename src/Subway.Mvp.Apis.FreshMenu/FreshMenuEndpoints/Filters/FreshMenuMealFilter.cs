using System.Diagnostics;
using System.Net;
using Subway.Mvp.Application.Features.FreshMenu.Meals;
using Subway.Mvp.Domain.FreshMenuVotes;

namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints.Filters;


public class FreshMenuMealFilter : IEndpointFilter
{
    public virtual async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // invalid meal
        string meal = context.GetArgument<string>(0);
        if (!string.IsNullOrWhiteSpace(meal) && !MealOfTheDayDto.GetAll().Any(x => x.Meal!.Equals(meal, StringComparison.Ordinal)))
        {
            return Results.Problem(FreshMenuVoteErrors.VoteNotAValidFreshMenuMealItemError.Code,
                $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}",
                (int)HttpStatusCode.BadRequest,
                FreshMenuVoteErrors.VoteNotAValidFreshMenuMealItemError.Description,
                type: $"{FreshMenuVoteErrors.VoteNotAValidFreshMenuMealItemError.Type}",
                extensions: new Dictionary<string, object?> { ["requestId"] = context.HttpContext.TraceIdentifier });
        }
        return await next(context);
    }
}
