using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using Subway.Mvp.Apis.FreshMenu;
using Subway.Mvp.Application.Abstractions;
using Subway.Mvp.Infrastructure.FreshMenu;

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
        List<FreshMenuIndexes.AllMeals.IndexEntry> meals =
            await session.Query<FreshMenuIndexes.AllMeals.IndexEntry, FreshMenuIndexes.AllMeals>()
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
        List<FreshMenuIndexes.MealByDayOfTheWeek.IndexEntry> meals =
            await session.Query<FreshMenuIndexes.MealByDayOfTheWeek.IndexEntry, FreshMenuIndexes.MealByDayOfTheWeek>()
                .Where(x => x.Day == tuesday)
                .ToListAsync();
        Assert.NotNull(session);
        Assert.NotNull(meals);
        Assert.Single(meals);
        FreshMenuIndexes.MealByDayOfTheWeek.IndexEntry meal = meals[0];
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
        List<FreshMenuIndexes.DayOfTheWeekByMeal.IndexEntry> meals =
            await session.Query<FreshMenuIndexes.DayOfTheWeekByMeal.IndexEntry, FreshMenuIndexes.DayOfTheWeekByMeal>()
                .Where(x => x.Meal == wednesdayMeal)
                .ToListAsync();
        Assert.NotNull(session);
        Assert.NotNull(meals);
        Assert.Single(meals);
        FreshMenuIndexes.DayOfTheWeekByMeal.IndexEntry meal = meals[0];
        Assert.NotNull(meal);
        Assert.Equal(wednesdayMeal, meal.Meal);
    }
}
