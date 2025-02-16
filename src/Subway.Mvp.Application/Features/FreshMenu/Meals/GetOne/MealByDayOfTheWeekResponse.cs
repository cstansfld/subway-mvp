namespace Subway.Mvp.Application.Features.FreshMenu.Meals.GetOne;

public sealed class MealByDayOfTheWeekResponse
{
    public required string Day { get; init; }
    public required string MealOfTheDay { get; init; }
    public required DateTime RequestDateTimeUtc { get; init; }
    public required DateTime DateTimeUtc { get; init; }

    public static MealByDayOfTheWeekResponse Create(string Day, string MealOfTheDay, DateTime RequestDateTimeUtc, DateTime DateTimeUtc)
    {
        return new MealByDayOfTheWeekResponse
        {
            Day = Day,
            MealOfTheDay = MealOfTheDay,
            RequestDateTimeUtc = RequestDateTimeUtc,
            DateTimeUtc = DateTimeUtc
        };
    }
}
