using Subway.Mvp.Application.Abstractions.Messaging;

namespace Subway.Mvp.Application.Features.FreshMenu.Meals.GetOne;

public sealed record GetMealByDayOfTheWeekQuery(DayOfWeek DayOfTheWeek, DateTime RequestDateTimeUtc) : IQuery<MealByDayOfTheWeekResponse>;
