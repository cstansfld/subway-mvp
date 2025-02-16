namespace Subway.Mvp.Application.Features.FreshMenu.Meals.GetAll;

public sealed class MealsOfTheDayResponse
{
    public DayOfWeek Day { get; set; }
    public string MealOfTheDay { get; set; }
}
