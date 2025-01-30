using Subway.Mvp.Domain.FreshMenu;

namespace Subway.Mvp.Application.Features.FreshMenu;

public sealed record MealOfTheDayDto(DateTime? DateTime, string? Meal)
{
    public string? Meal { get; init; } = Meal;
    public string? FeatureMealByDate { get; init; }
    public DateTime UtcDateTime { get; init; }
    public required DayOfWeek Day { get; init; } = DateTime?.DayOfWeek ?? System.DateTime.UtcNow.DayOfWeek;
    public required string TodaysMeal { get; init; } = MealOfTheDay.MealOfToday.Meal;
    public bool IsMealTodaysFeatureMealOfDay { get; init; }
    public bool IsMealFeatureMealByDate { get; init; }

    public static implicit operator MealOfTheDayDto(MealOfTheDay dom) => new(System.DateTime.UtcNow, dom.Meal)
    {
        Day = dom.Day,
        Meal = dom.Meal,
        FeatureMealByDate = MealOfTheDay.GetMealOfTheDayByUtcDate(System.DateTime.UtcNow).Meal,
        TodaysMeal = MealOfTheDay.MealOfToday.Meal,
        IsMealTodaysFeatureMealOfDay = MealOfTheDay.MealOfToday.Meal == dom.Meal,
        IsMealFeatureMealByDate = MealOfTheDay.GetMealOfTheDayByUtcDate(System.DateTime.UtcNow).Meal.Equals(dom.Meal, StringComparison.OrdinalIgnoreCase)
    };

    public static MealOfTheDayDto Get(DateTime? DateTime, string? Meal)
    {
        return new(DateTime, Meal ?? MealOfTheDay.MealOfToday.Meal)
        {
            Meal = Meal,
            FeatureMealByDate = MealOfTheDay.GetMealOfTheDayByUtcDate(DateTime ?? System.DateTime.UtcNow).Meal,
            Day = DateTime?.DayOfWeek ?? System.DateTime.UtcNow.DayOfWeek,
            UtcDateTime = DateTime ?? System.DateTime.UtcNow,
            TodaysMeal = MealOfTheDay.MealOfToday.Meal,
            IsMealTodaysFeatureMealOfDay = MealOfTheDay.MealOfToday.Meal.Equals(Meal ?? MealOfTheDay.GetMealOfTheDayByUtcDate(DateTime ?? System.DateTime.UtcNow).Meal, StringComparison.OrdinalIgnoreCase),
            IsMealFeatureMealByDate = MealOfTheDay.GetMealOfTheDayByUtcDate(DateTime ?? System.DateTime.UtcNow).Meal.Equals(Meal, StringComparison.OrdinalIgnoreCase)
        };
    }

    public static MealOfTheDayDto[] GetAll()
    {
        return [MealOfTheDay.Monday, MealOfTheDay.Tuesday, MealOfTheDay.Wednesday,
            MealOfTheDay.Thursday, MealOfTheDay.Friday, MealOfTheDay.Saturday, MealOfTheDay.Sunday];
    }
}
