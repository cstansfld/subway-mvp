using System.Globalization;
using System.Net.Http.Json;
using Subway.Mvp.Application.Features.FreshMenu.Meals;
using Subway.Mvp.Application.Features.FreshMenu.Meals.Get;
using Subway.Mvp.Application.Features.FreshMenu.Meals.GetAll;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Tests.Endpoints;

[Collection("FreshMenu Collection")]
public class FreshMenuTests : BaseFreshMenuFixture
{
    public FreshMenuTests(FreshMenuIntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task FreshMenu_Endpoint_Success()
    {
        using HttpClient client = Factory.CreateClient();
        Result<List<MealsOfTheDayResponse>> response = await client.GetFromJsonAsync<Result<List<MealsOfTheDayResponse>>>($"v1/freshmenu/all");
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equivalent(expected: response.Value.Select(x => x.MealOfTheDay), actual: MealOfTheDayDto.GetAll().Select(x => x.Meal!), strict: false);
    }

    [Fact]
    public async Task FreshMenu_Endpoint_GetMealsOfTheDay_Query()
    {
        Result<List<MealsOfTheDayResponse>> result = await Sender.Send(new GetMealsOfTheDayQuery());
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equivalent(expected: result.Value.Select(x => x.MealOfTheDay), actual: MealOfTheDayDto.GetAll().Select(x => x.Meal!), strict: false);
    }

    [Fact]
    public async Task FreshMenu_Endpoint_GetMealOfTheDay_DateNow_Query()
    {
        DateTime utcNow = DateTime.UtcNow;
        var query = new GetMealOfTheDayQuery(DateTime.UtcNow, default);
        Result<MealOfTheDayDto> result = await Sender.Send(query);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Day, utcNow.DayOfWeek);
        Assert.True(result.Value.IsMealTodaysFeatureMealOfDay);
    }

    [Fact]
    public async Task FreshMenu_Endpoint_GetMealOfTheDay_DateNow_Meal_EqualsFeature_Meal_Query()
    {
        DayOfWeek thursDay = DayOfWeek.Thursday;
        var utcDate = DateTime.Parse("2004-09-16T23:59:58.75565", CultureInfo.InvariantCulture);
        const string Meal = "All-New Baja Chipotle Chicken";

        var query = new GetMealOfTheDayQuery(utcDate, Meal);
        Result<MealOfTheDayDto> result = await Sender.Send(query);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Day, thursDay);
        Assert.Equal(Meal, result.Value.Meal, ignoreCase: true);
        if (DateTime.UtcNow.DayOfWeek == thursDay)
        {
            Assert.True(result.Value.IsMealTodaysFeatureMealOfDay);//if today is thursDay this will fail
        }
        else
        {
            Assert.False(result.Value.IsMealTodaysFeatureMealOfDay);
        }
        Assert.True(result.Value.IsMealFeatureMealByDate);
    }

    [Fact]
    public async Task FreshMenu_Endpoint_GetMealOfTheDay_DateNow_Meal_DoesNotEqualFeature_Meal_Query()
    {
        DayOfWeek wednesday = DayOfWeek.Wednesday;
        var utcDate = DateTime.Parse("2004-09-15", CultureInfo.InvariantCulture);
        const string Meal = "meatball marinara";

        var query = new GetMealOfTheDayQuery(utcDate, Meal);
        Result<MealOfTheDayDto> result = await Sender.Send(query);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Day, wednesday);
        Assert.Equal(Meal, result.Value.Meal, ignoreCase: true);
        if (DateTime.UtcNow.DayOfWeek == wednesday)
        {
            Assert.True(result.Value.IsMealTodaysFeatureMealOfDay);//if today is wednesday this will fail
        }
        else
        {
            Assert.False(result.Value.IsMealTodaysFeatureMealOfDay);
        }
        Assert.True(result.Value.IsMealFeatureMealByDate);
    }
}
