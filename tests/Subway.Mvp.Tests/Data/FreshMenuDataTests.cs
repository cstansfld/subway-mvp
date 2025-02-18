using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Domain.FreshMenuVotes;

namespace Subway.Mvp.Tests.Data;

[Collection("FreshMenu Collection")]
public class FreshMenuDataTests : BaseFreshMenuFixture
{
    public FreshMenuDataTests(FreshMenuIntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task FreshMenu_Data_Query_For_All_Fresh_Menu_Meals()
    {
        // Arrange
        await InitAllMeals();
        // Act
        using IAsyncDocumentSession session = DocumentStoreContainer.Store.OpenAsyncSession();
        List<AllMeals.IndexEntry> meals =
            await session.Query<AllMeals.IndexEntry, AllMeals>()
            .ToListAsync(CancellationToken.None);
        // Assert
        Assert.NotNull(session);
        Assert.NotNull(meals);
        Assert.Equal(7, meals.Count);
    }

    [Fact]
    public async Task FreshMenu_Data_Query_For_A_Fresh_Menu_Meal_On_Tuesday()
    {
        // Arrange
        await InitAllMeals();
        // Act
        DayOfWeek tuesday = DayOfWeek.Tuesday;
        using IAsyncDocumentSession session = DocumentStoreContainer.Store.OpenAsyncSession();
        List<MealByDayOfTheWeek.IndexEntry> meals =
            await session.Query<MealByDayOfTheWeek.IndexEntry, MealByDayOfTheWeek>()
                .Where(x => x.Day == tuesday)
                .ToListAsync();
        // Assert
        Assert.NotNull(session);
        Assert.NotNull(meals);
        Assert.Single(meals);
        MealByDayOfTheWeek.IndexEntry meal = meals[0];
        Assert.NotNull(meal);
        Assert.Equal(tuesday, meal.Day);
    }

    [Fact]
    public async Task FreshMenu_Data_Query_For_A_Fresh_Menu_Meal_By_Name_Meatball_Marinara()
    {
        // Arrange
        await InitAllMeals();
        // Act
        string wednesdayMeal = "Meatball Marinara";
        using IAsyncDocumentSession session = DocumentStoreContainer.Store.OpenAsyncSession();
        List<DayOfTheWeekByMeal.IndexEntry> meals =
            await session.Query<DayOfTheWeekByMeal.IndexEntry, DayOfTheWeekByMeal>()
                .Where(x => x.Meal == wednesdayMeal)
                .ToListAsync();
        //Assert
        Assert.NotNull(session);
        Assert.NotNull(meals);
        Assert.Single(meals);
        DayOfTheWeekByMeal.IndexEntry meal = meals[0];
        Assert.NotNull(meal);
        Assert.Equal(wednesdayMeal, meal.Meal);
    }


    [Theory]
    [InlineData("The Philly")]
    [InlineData("Cold Cut Combo")]
    [InlineData("All-Pro Sweet Onion Chicken Teriyaki")]
    [InlineData("Meatball Marinara")]
    [InlineData("All-New Baja Chipotle Chicken")]
    [InlineData("Tuna")]
    [InlineData("The Ultimate B.M.T.")]
    public async Task FreshMenu_Data_Vote_For_A_Fresh_Menu_Meal_By_Fresh_Meal_Name(string freshMealName)
    {
        // Arrange
        await InitAllVotes();
        // Act
        List<AllVotes.IndexEntry> beforeVote = await AppDbContext.GetAllFreshMenuVotes();
        int beforeVoteCount = beforeVote.First(x => x.Meal == freshMealName).VotedFor;
        FreshMenuVote voteplaced = await AppDbContext.VoteForFreshMenuMeal(freshMealName);
        List<AllVotes.IndexEntry> afterVote = await AppDbContext.GetAllFreshMenuVotes();
        int afterVoteCount = afterVote.First(x => x.Meal == freshMealName).VotedFor;
        // Assert
        Assert.NotNull(beforeVote);
        Assert.NotNull(voteplaced);
        Assert.NotNull(afterVote);
        Assert.Equal(beforeVoteCount + 1, voteplaced.VotedFor);
        Assert.Equal(afterVoteCount, voteplaced.VotedFor);
    }

    [Fact]
    public async Task FreshMenu_Data_Vote_Summary()
    {
        // Arrange
        await InitAllVotes();
        await SetAllVotes();
        // Act
        using IAsyncDocumentSession session = DocumentStoreContainer.Store.OpenAsyncSession();
        List<AllVotes.IndexEntry> voteSummary =
            await session.Query<AllVotes.IndexEntry, AllVotes>()
            .ToListAsync();
        // Asssert
        Assert.NotNull(voteSummary);
        int total = voteSummary.Sum(x => x.VotedFor);
        var results = (from x in voteSummary
                       select new
                       {
                           Total = total,
                           Category = x.Meal,
                           x.VotedFor,
                           AsPercentOf = total > 0 ? ((decimal)x.VotedFor / total * 100) : 0
                       }).ToList();
        Assert.NotNull(results);
        Assert.True(results.Sum(x => x.AsPercentOf) > 99);
    }

    private async Task InitAllMeals(CancellationToken cancellationToken = default)
    {
        using IAsyncDocumentSession session = DocumentStoreContainer.Store.OpenAsyncSession();
        await session.StoreAsync(MealOfTheDay.Monday, "MealsOfTheDay/Monday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Tuesday, "MealsOfTheDay/Tuesday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Wednesday, "MealsOfTheDay/Wednesday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Thursday, "MealsOfTheDay/Thursday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Friday, "MealsOfTheDay/Friday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Saturday, "MealsOfTheDay/Saturday", cancellationToken);
        await session.StoreAsync(MealOfTheDay.Sunday, "MealsOfTheDay/Sunday", cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }

    private async Task InitAllVotes(CancellationToken cancellationToken = default)
    {
        using IAsyncDocumentSession session = DocumentStoreContainer.Store.OpenAsyncSession();
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Sunday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Sunday.Meal}", cancellationToken);
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Monday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Monday.Meal}", cancellationToken);
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Tuesday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Tuesday.Meal}", cancellationToken);
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Wednesday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Wednesday.Meal}", cancellationToken);
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Thursday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Thursday.Meal}", cancellationToken);
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Friday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Friday.Meal}", cancellationToken);
        await session.StoreAsync(new FreshMenuVote() { Meal = MealOfTheDay.Saturday.Meal, VotedFor = 0 },
            $"FreshMenuVotes/{MealOfTheDay.Saturday.Meal}", cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }


    private async Task<List<FreshMenuVote>> SetAllVotes()
    {
        var votes = new List<FreshMenuVote>();
        var meals = new List<string> { "The Philly", "Cold Cut Combo", "All-Pro Sweet Onion Chicken Teriyaki",
            "Meatball Marinara", "All-New Baja Chipotle Chicken", "Tuna", "The Ultimate B.M.T." };
        foreach (string meal in meals)
        {
            FreshMenuVote voteplaced = await AppDbContext.VoteForFreshMenuMeal(meal);
            votes.Add(voteplaced);
        }
        return votes;
    }
}
