using Raven.Client.Documents.Indexes;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Domain.FreshMenuVotes;
using Subway.Mvp.Shared;

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


    public sealed class AllMeals : AbstractIndexCreationTask<MealOfTheDay>
    {
        public sealed class IndexEntry
        {
            // The index-fields:
            public string Meal { get; set; }
            public DayOfWeek Day { get; set; }
        }

        public AllMeals()
        {
            Map = mealsOfTheDay => from mealOfTheDay in mealsOfTheDay
                                   select new IndexEntry() { Day = mealOfTheDay.Day, Meal = mealOfTheDay.Meal };
            DeploymentMode = IndexDeploymentMode.Rolling;
            Configuration = new IndexConfiguration
            {
                { "Indexing.IndexMissingFieldsAsNull", "true" }
            };
        }
    }


    public sealed class MealByDayOfTheWeek : AbstractIndexCreationTask<MealOfTheDay>
    {
        public sealed class IndexEntry
        {
            public string Meal { get; set; }
            // The index-fields
            public DayOfWeek Day { get; set; }
        }

        public MealByDayOfTheWeek()
        {
            Map = mealsOfTheDay => from mealOfTheDay in mealsOfTheDay
                                   select new IndexEntry()
                                   {
                                       Meal = mealOfTheDay.Meal,
                                       // Define the content for each index-field
                                       Day = mealOfTheDay.Day
                                   };
            DeploymentMode = IndexDeploymentMode.Rolling;
            Configuration = new IndexConfiguration
            {
                { "Indexing.IndexMissingFieldsAsNull", "true" }
            };
        }
    }

    public sealed class DayOfTheWeekByMeal : AbstractIndexCreationTask<MealOfTheDay>
    {
        public sealed class IndexEntry
        {
            // The index-fields
            public string Meal { get; set; }
            public DayOfWeek Day { get; set; }
        }

        public DayOfTheWeekByMeal()
        {
            Map = mealsOfTheDay => from mealOfTheDay in mealsOfTheDay
                                   select new IndexEntry()
                                   {
                                       // Define the content for each index-field
                                       Meal = mealOfTheDay.Meal,
                                       Day = mealOfTheDay.Day
                                   };
            DeploymentMode = IndexDeploymentMode.Rolling;
            Configuration = new IndexConfiguration
            {
                { "Indexing.IndexMissingFieldsAsNull", "true" }
            };
        }
    }


    public sealed class AllVotes : AbstractIndexCreationTask<FreshMenuVote>
    {
        public sealed class VoteIndex
        {
            // The index-fields:
            public string Meal { get; set; }
            public int VotedFor { get; set; }
        }

        public AllVotes()
        {
            Map = freshMenuVotes => from freshMenuVote in freshMenuVotes
                                    select new VoteIndex()
                                    { 
                                        Meal = freshMenuVote.Meal,
                                        VotedFor = freshMenuVote.VotedFor
                                    };
            DeploymentMode = IndexDeploymentMode.Rolling;
            Configuration = new IndexConfiguration
            {
                { "Indexing.IndexMissingFieldsAsNull", "true" }
            };
        }
    }
}
