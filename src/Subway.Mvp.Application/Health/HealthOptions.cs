using Microsoft.Extensions.Options;

namespace Subway.Mvp.Application.Health;

public class HealthOptions
{
    public required string HealthHost { get; init; } = string.Empty;
}

public class HealthOptionsSetup(IConfiguration _configuration) : IConfigureOptions<HealthOptions>
{
    public static readonly string SectionName = "HealthOptions";

    public void Configure(HealthOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
