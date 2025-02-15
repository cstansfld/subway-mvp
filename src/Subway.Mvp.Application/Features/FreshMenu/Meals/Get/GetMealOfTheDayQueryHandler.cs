﻿using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Features.FreshMenu.Meals.Get;

internal sealed class GetMealOfTheDayQueryHandler()
    : IQueryHandler<GetMealOfTheDayQuery, MealOfTheDayDto>
{
    public async Task<Result<MealOfTheDayDto>> Handle(GetMealOfTheDayQuery query, CancellationToken cancellationToken)
    {
        MealOfTheDayDto mealoftheday = await Task.FromResult(
            MealOfTheDayDto.Get(query.DateTime, query.Meal));

        return mealoftheday;
    }
}
