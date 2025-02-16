using Subway.Mvp.Shared;
using Subway.Mvp.Application.Abstractions.Messaging;

namespace Subway.Mvp.Application.Features.FreshMenu.Meals.Get;

public sealed record GetMealOfTheDayQuery(DateTime DateTime, string? Meal) : IQuery<MealOfTheDayDto>;

