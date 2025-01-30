using Subway.Mvp.Domain.FreshMenu;

namespace Subway.Mvp.Application.Features.FreshMenu;

public sealed record MealOfTheDayDto(DateTime? DateTime, string? Meal)
{
    public string? Meal { get; init; } = Meal ?? MealOfTheDay.MealOfToday.Meal;
    public string? FeatureMeal { get; init; } = MealOfTheDay.MealOfToday.Meal;
    public DateTime UtcDateTime { get; init; } = DateTime ?? System.DateTime.UtcNow;
    public required DayOfWeek Day { get; init; } = DateTime?.DayOfWeek ?? System.DateTime.UtcNow.DayOfWeek;
    public bool IsMealFeatureMealOfDay { get; init; } = MealOfTheDay.MealOfToday.Meal == Meal;

    public static implicit operator MealOfTheDayDto(MealOfTheDay dom) => new(System.DateTime.UtcNow, dom.Meal)
    {
        Day = dom.Day,
        FeatureMeal = MealOfTheDay.MealOfToday.Meal,
        Meal = dom.Meal ?? MealOfTheDay.MealOfToday.Meal,
        IsMealFeatureMealOfDay = MealOfTheDay.MealOfToday.Meal == dom.Meal
    };

    public static MealOfTheDayDto Create(DateTime? DateTime, string? Meal)
    {
        return new(DateTime, Meal ?? MealOfTheDay.MealOfToday.Meal)
        {
            Meal = Meal,
            FeatureMeal = MealOfTheDay.MealOfToday.Meal,
            Day = DateTime?.DayOfWeek ?? System.DateTime.UtcNow.DayOfWeek,
            UtcDateTime = DateTime ?? System.DateTime.UtcNow,
            IsMealFeatureMealOfDay = MealOfTheDay.MealOfToday.Meal == Meal
        };
    }

    public static MealOfTheDayDto[] GetAll()
    {
        return [MealOfTheDay.Monday, MealOfTheDay.Tuesday, MealOfTheDay.Wednesday,
            MealOfTheDay.Thursday, MealOfTheDay.Friday, MealOfTheDay.Saturday, MealOfTheDay.Sunday];
    }
}
