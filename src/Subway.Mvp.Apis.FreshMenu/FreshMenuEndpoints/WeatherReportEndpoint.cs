namespace Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints;

public static class WeatherReportEndpoint
{
    private static readonly string[] summaries = [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

    public static void MapWeatherReportEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/weatherforecast/{test}", async (bool test) =>
        {
            if (test)
            {
                return await Task.FromResult(summaries);
            }

            throw new InvalidOperationException("This is a weatherforecast exception message.");
        }).CacheOutput("Expire60mins").WithName("GetWeatherForecast").WithOpenApi();
    }
}
