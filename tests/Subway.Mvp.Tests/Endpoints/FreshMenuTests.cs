using System.Net.Http.Json;
using Subway.Mvp.Apis.FreshMenu;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Application.Features.FreshMenu.GetAll;
using Subway.Mvp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Subway.Mvp.Tests.Endpoints;

public class FreshMenuTests
{
    [Fact]
    public async Task FreshMenu_Endpoint_Success()
    {
        await using var application = new WebApplicationFactory<Program>();
        using HttpClient client = application.CreateClient();
        Result<List<MealsOfTheDayResponse>> response = await client.GetFromJsonAsync<Result<List<MealsOfTheDayResponse>>>($"/freshmenu/all");
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equivalent(expected: response.Value.Select(x => x.MealOfTheDay), actual: MealOfTheDayDto.GetAll().Select(x => x.Meal!), strict: false);
    }
}
