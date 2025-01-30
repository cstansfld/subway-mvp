using Subway.Mvp.Shared;

namespace Subway.Mvp.Domain.FreshMenu;

public static class MealOfTheDayErrors
{
    public static readonly Error InvalidMealError = new("MealOfTheDay.InvalidMealError", "Invalid meal entered.", ErrorType.Problem);
}
