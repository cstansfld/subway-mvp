using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using Subway.Mvp.Apis.FreshMenu;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Domain.FreshMenuVotes;

namespace Subway.Mvp.Tests.Data;
public class FreshMenuDataTests
{
    [Fact]
    public async Task FreshMenu_Data_Query_For_All_Fresh_Menu_Meals()
    {
        await using var application = new WebApplicationFactory<Program>();
        IServiceProvider _serviceProvider = application.Services;
        IServiceScopeFactory _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        IDocumentStoreContainer documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStoreContainer>();
        using IAsyncDocumentSession session = documentStore.Store.OpenAsyncSession();
        List<AllMeals.IndexEntry> meals =
            await session.Query<AllMeals.IndexEntry, AllMeals>()
            .ToListAsync();
        Assert.NotNull(session);
        Assert.NotNull(meals);
        Assert.Equal(7, meals.Count);
    }

    [Fact]
    public async Task FreshMenu_Data_Query_For_A_Fresh_Menu_Meal_On_Tuesday()
    {
        DayOfWeek tuesday = DayOfWeek.Tuesday;
        await using var application = new WebApplicationFactory<Program>();
        IServiceProvider _serviceProvider = application.Services;
        IServiceScopeFactory _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        IDocumentStoreContainer documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStoreContainer>();
        using IAsyncDocumentSession session = documentStore.Store.OpenAsyncSession();
        List<MealByDayOfTheWeek.IndexEntry> meals =
            await session.Query<MealByDayOfTheWeek.IndexEntry, MealByDayOfTheWeek>()
                .Where(x => x.Day == tuesday)
                .ToListAsync();
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
        string wednesdayMeal = "Meatball Marinara";
        await using var application = new WebApplicationFactory<Program>();
        IServiceProvider _serviceProvider = application.Services;
        IServiceScopeFactory _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        IDocumentStoreContainer documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStoreContainer>();
        using IAsyncDocumentSession session = documentStore.Store.OpenAsyncSession();
        List<DayOfTheWeekByMeal.IndexEntry> meals =
            await session.Query<DayOfTheWeekByMeal.IndexEntry, DayOfTheWeekByMeal>()
                .Where(x => x.Meal == wednesdayMeal)
                .ToListAsync();
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
        await using var application = new WebApplicationFactory<Program>();
        IServiceProvider _serviceProvider = application.Services;
        IApplicationDbContext _applicationDbContext = _serviceProvider.GetRequiredService<IApplicationDbContext>();

        List<AllVotes.IndexEntry> beforeVote = await _applicationDbContext.GetAllFreshMenuVotes();
        int beforeVoteCount = beforeVote.First(x => x.Meal == freshMealName).VotedFor;
        FreshMenuVote voteplaced = await _applicationDbContext.VoteForFreshMenuMeal(freshMealName);
        List<AllVotes.IndexEntry> afterVote = await _applicationDbContext.GetAllFreshMenuVotes();
        int afterVoteCount = afterVote.First(x => x.Meal == freshMealName).VotedFor;
        Assert.NotNull(beforeVote);
        Assert.NotNull(voteplaced);
        Assert.NotNull(afterVote);
        Assert.Equal(beforeVoteCount + 1, voteplaced.VotedFor);
        Assert.Equal(afterVoteCount, voteplaced.VotedFor);
    }

    [Fact]
    public async Task FreshMenu_Data_Vote_Summary()
    {
        await using var application = new WebApplicationFactory<Program>();
        IServiceProvider _serviceProvider = application.Services;
        IServiceScopeFactory _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        IDocumentStoreContainer documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStoreContainer>();
        using IAsyncDocumentSession session = documentStore.Store.OpenAsyncSession();
        List<AllVotes.IndexEntry> voteSummary =
            await session.Query<AllVotes.IndexEntry, AllVotes>()
            .ToListAsync();
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
}
