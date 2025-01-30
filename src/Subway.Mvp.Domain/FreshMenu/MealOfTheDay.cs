namespace Subway.Mvp.Domain.FreshMenu;

public sealed class MealOfTheDay
{
    public required DayOfWeek Day { get; set; }
    public required string Meal { get; set; }

    public static MealOfTheDay Monday => Create(DayOfWeek.Monday, "Cold Cut Combo");
    public static MealOfTheDay Tuesday => Create(DayOfWeek.Tuesday, "All-Pro Sweet Onion Chicken Teriyaki");
    public static MealOfTheDay Wednesday => Create(DayOfWeek.Wednesday, "Meatball Marinara");
    public static MealOfTheDay Thursday => Create(DayOfWeek.Thursday, "All-New Baja Chipotle Chicken");
    public static MealOfTheDay Friday => Create(DayOfWeek.Friday, "Tuna");
    public static MealOfTheDay Saturday => Create(DayOfWeek.Saturday, "The Ultimate B.M.T.");
    public static MealOfTheDay Sunday => Create(DayOfWeek.Sunday, "The Philly");

    public static MealOfTheDay MealOfToday => DateTime.UtcNow.DayOfWeek switch
    {
        DayOfWeek.Monday => Monday,
        DayOfWeek.Tuesday => Tuesday,
        DayOfWeek.Wednesday => Wednesday,
        DayOfWeek.Thursday => Thursday,
        DayOfWeek.Friday => Friday,
        DayOfWeek.Saturday => Saturday,
        DayOfWeek.Sunday => Sunday,
        _ => Monday
    };

    public static MealOfTheDay GetMealOfTheDayByUtcDate(DateTime utcDateTime)
    {
        return utcDateTime.DayOfWeek switch
        {
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            DayOfWeek.Sunday => Sunday,
            _ => Monday,
        };
    }

    public static MealOfTheDay Create(DayOfWeek? dayOfWeek, string? meal) => new()
    {
        Day = dayOfWeek ?? DateTime.UtcNow.DayOfWeek,
        Meal = meal ?? MealOfToday.Meal
    };
}
