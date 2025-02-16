using Raven.Client.Documents.Indexes;
using Subway.Mvp.Application.Features.FreshMenu;

namespace Subway.Mvp.Infrastructure.FreshMenu;

public static class FreshMenuIndexes
{
    /// <summary>
    /// GetAllFreshMenuIndexes
    /// </summary>
    /// <returns></returns>
    public static List<AbstractIndexCreationTask> GetAllFreshMenuIndexes()
        =>
        [
            new AllMeals(),
            new MealByDayOfTheWeek(),
            new DayOfTheWeekByMeal(),
            new AllVotes()
        ];
}
