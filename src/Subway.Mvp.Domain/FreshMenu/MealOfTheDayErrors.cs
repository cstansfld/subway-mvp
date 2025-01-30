using Subway.Mvp.Shared;

namespace Subway.Mvp.Domain.FreshMenu;

public static class MealOfTheDayErrors
{
    public static readonly Error InvalidMealError = new("MealOfTheDay.InvalidMealError", "FreshMenu invalid meal.", ErrorType.Problem);
    public static readonly Error InvalidDateError = new("MealOfTheDay.InvalidDateError", "FreshMenu invalid date.", ErrorType.Problem);
}
