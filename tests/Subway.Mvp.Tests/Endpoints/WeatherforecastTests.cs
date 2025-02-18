using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Subway.Mvp.Tests.Endpoints;

[Collection("FreshMenu Collection")]
public class WeatherforecastTests : BaseFreshMenuFixture
{
    public WeatherforecastTests(FreshMenuIntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Weatherforecast_Endpoint_Success()
    {
        using HttpClient client = Factory.CreateClient();

        string[] response = await client.GetFromJsonAsync<string[]>($"v1/weatherforecast/{true}");

        Assert.NotNull(response);
        Assert.Equal(["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"], response);
    }

    [Fact]
    public async Task Weatherforecast_Endpoint_ProblemDetail()
    {
        const string DataTrackerIetfInternalServerError = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
        const string WeatherForecastDetailExceptionMessage = "This is a weatherforecast exception message.";
        const int WeatherForecastExceptionStatusCode = 500;

        using HttpClient client = Factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"v1/weatherforecast/{false}");

        Assert.NotNull(response);
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError); // statuscode 500

        ProblemDetails? problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(await ReadResponseAsString(response));

        Assert.NotNull(problemDetails);
        Assert.Equal(DataTrackerIetfInternalServerError, problemDetails.Type);
        Assert.Equal(WeatherForecastExceptionStatusCode, problemDetails.Status);
        Assert.Equal(WeatherForecastDetailExceptionMessage, problemDetails.Detail);
    }

    private static async Task<string> ReadResponseAsString(HttpResponseMessage response)
    {
        byte[] byteArray = await response.Content.ReadAsByteArrayAsync();
        return Encoding.UTF8.GetString(byteArray);
    }
}
