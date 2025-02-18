using Newtonsoft.Json;

namespace Subway.Mvp.Tests.Endpoints;

[Collection("FreshMenu Collection")]
public class HealthTests : BaseFreshMenuFixture
{
    private readonly string[] StatusValues = ["Healthy", "Degraded", "Unhealthy"];

    public HealthTests(FreshMenuIntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Health_Endpoint_Check_Status()
    {
        using HttpClient client = Factory.CreateClient();

        string response = await client.GetStringAsync($"/health");
        Assert.NotNull(response);
        var result = JsonConvert.DeserializeAnonymousType(response, new { Status = "" });
        Assert.NotNull(result);
        Assert.Contains(result.Status, StatusValues);
    }
}
