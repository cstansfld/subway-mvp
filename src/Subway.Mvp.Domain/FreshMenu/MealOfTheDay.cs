namespace Subway.Mvp.Domain.FreshMenu;

public sealed class MealOfTheDay
{
    public required DayOfWeek Day { get; set; }
    public required string Meal { get; set; }

    public static MealOfTheDay Sunday => Create(DayOfWeek.Sunday, "The Philly");
    public static MealOfTheDay Monday => Create(DayOfWeek.Monday, "Cold Cut Combo");
    public static MealOfTheDay Tuesday => Create(DayOfWeek.Tuesday, "All-Pro Sweet Onion Chicken Teriyaki");
    public static MealOfTheDay Wednesday => Create(DayOfWeek.Wednesday, "Meatball Marinara");
    public static MealOfTheDay Thursday => Create(DayOfWeek.Thursday, "All-New Baja Chipotle Chicken");
    public static MealOfTheDay Friday => Create(DayOfWeek.Friday, "Tuna");
    public static MealOfTheDay Saturday => Create(DayOfWeek.Saturday, "The Ultimate B.M.T.");

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

    public static (string Day, MealOfTheDay Meal) GetMealByDayInfo(DayOfWeek day)
    {
        var dayNames = new Dictionary<DayOfWeek, (string, MealOfTheDay)>
        {
            { DayOfWeek.Sunday, ("Sunday", Sunday) },
            { DayOfWeek.Monday, ("Monday", Monday) },
            { DayOfWeek.Tuesday, ("Tuesday", Tuesday) },
            { DayOfWeek.Wednesday, ("Wednesday", Wednesday) },
            { DayOfWeek.Thursday, ("Thursday", Thursday) },
            { DayOfWeek.Friday, ("Friday", Friday) },
            { DayOfWeek.Saturday, ("Saturday", Saturday) }
        };

        return dayNames[day];
    }

    public static string GetDayName(DayOfWeek day)
    {
        var dayNames = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Sunday, "Sunday" },
            { DayOfWeek.Monday, "Monday" },
            { DayOfWeek.Tuesday, "Tuesday" },
            { DayOfWeek.Wednesday, "Wednesday" },
            { DayOfWeek.Thursday, "Thursday" },
            { DayOfWeek.Friday, "Friday" },
            { DayOfWeek.Saturday, "Saturday" }
        };

        return dayNames[day];
    }
}
