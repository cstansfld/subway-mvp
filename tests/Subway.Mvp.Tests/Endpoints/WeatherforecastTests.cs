using System.Net;
using System.Net.Http.Json;
using System.Text;
using Subway.Mvp.Apis.FreshMenu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Subway.Mvp.Tests.Endpoints;

public class WeatherforecastTests
{
    [Fact]
    public async Task Weatherforecast_Endpoint_Success()
    {
        await using var application = new WebApplicationFactory<Program>();
        using HttpClient client = application.CreateClient();

        string[] response = await client.GetFromJsonAsync<string[]>($"/weatherforecast/{true}");

        Assert.NotNull(response);
        Assert.Equal(["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"], response);
    }

    [Fact]
    public async Task Weatherforecast_Endpoint_ProblemDetail()
    {
        await using var application = new WebApplicationFactory<Program>();
        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/weatherforecast/{false}");

        Assert.NotNull(response);
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError); // statuscode 500

        ProblemDetails? problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(await ReadResponseAsString(response));

        Assert.NotNull(problemDetails);
        Assert.Equal("https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1", problemDetails.Type);
    }

    private static async Task<string> ReadResponseAsString(HttpResponseMessage response)
    {
        byte[] byteArray = await response.Content.ReadAsByteArrayAsync();
        return Encoding.UTF8.GetString(byteArray);
    }
}
