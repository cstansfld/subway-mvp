using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Application.Features.FreshMenu.Get;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Features.FreshMenu.GetAll;

internal sealed class GetMealsOfTheDayQueryHandler()
    : IQueryHandler<GetMealsOfTheDayQuery, List<MealsOfTheDayResponse>>
{
    public async Task<Result<List<MealsOfTheDayResponse>>> Handle(GetMealsOfTheDayQuery query, CancellationToken cancellationToken)
    {
        var mealsoftheday = await Task.FromResult(
            MealOfTheDayDto.GetAll().Select(x => new MealsOfTheDayResponse() { Day = x.Day, MealOfTheDay = x.Meal! }).ToList());

        return mealsoftheday;
    }
}
