using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Subway.Mvp.Apis.FreshMenu;

namespace Subway.Mvp.Tests.Endpoints;

public class HealthTests
{
    private readonly string[] StatusValues = ["Healthy", "Degraded"];

    [Fact]
    public async Task Health_Endpoint_Check_Status()
    {
        await using var application = new WebApplicationFactory<Program>();
        using HttpClient client = application.CreateClient();

        string response = await client.GetStringAsync($"/health");
        Assert.NotNull(response);
        var result = JsonConvert.DeserializeAnonymousType(response, new { Status = "" });
        Assert.NotNull(result);
        Assert.Contains(result.Status, StatusValues);
    }
}
