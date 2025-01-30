using Subway.Mvp.Apis.FreshMenu;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Subway.Mvp.Tests.Endpoints;

public class HealthTests
{
    [Fact]
    public async Task Health_Endpoint_Check_Healthy()
    {
        await using var application = new WebApplicationFactory<Program>();
        using HttpClient client = application.CreateClient();

        string response = await client.GetStringAsync($"/health");
        Assert.NotNull(response);
        var result = JsonConvert.DeserializeAnonymousType(response, new { Status = "" });
        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);

    }
}
