using Raven.Client.Documents.Indexes;
using Subway.Mvp.Domain.FreshMenu;

namespace Subway.Mvp.Infrastructure.FreshMenu;

internal static class FreshMenuIndexes
{
    public sealed class MealByDayOfTheWeek : AbstractIndexCreationTask<MealOfTheDay>
    {
        public sealed class IndexEntry
        {
            // The index-fields
            public int Day { get; set; }
        }

        public MealByDayOfTheWeek()
        {
            Map = mealsOfTheDay => from mealOfTheDay in mealsOfTheDay
                                   select new IndexEntry()
                                   {
                                       // Define the content for each index-field
                                       Day = TryConvert<int>(mealOfTheDay.Day) ?? -1 // nullable in this case is protected by seeding (static lookup)
                                   };
        }
    }

    public sealed class DayOfTheWeekByMeal : AbstractIndexCreationTask<MealOfTheDay>
    {
        public sealed class IndexEntry
        {
            // The index-fields
            public string Meal { get; set; }
        }

        public DayOfTheWeekByMeal()
        {
            Map = mealsOfTheDay => from mealOfTheDay in mealsOfTheDay
                                   select new IndexEntry()
                                   {
                                       // Define the content for each index-field
                                       Meal = mealOfTheDay.Meal
                                   };
        }
    }


}
