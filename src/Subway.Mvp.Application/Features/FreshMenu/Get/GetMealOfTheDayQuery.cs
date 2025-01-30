using Subway.Mvp.Shared;
using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Application.Features.FreshMenu;

namespace Subway.Mvp.Application.Features.FreshMenu.Get;

public sealed record GetMealOfTheDayQuery(DateTime DateTime, string? Meal) : IQuery<MealOfTheDayDto>;

