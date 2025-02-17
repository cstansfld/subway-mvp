using Microsoft.Extensions.Options;

namespace Subway.Mvp.Application.Features.FreshMenu;

public class FreshMenuStorageOptions
{
    public required string DataDirectory { get; init; } = "./.containers/freshmenu/v1";
    public required string DatabaseName { get; init; } = "FreshMenuDb";
    public required string ServerUrl { get; init; } = string.Empty;
}

public class FreshMenuStorageSetup(IConfiguration _configuration) : IConfigureOptions<FreshMenuStorageOptions>
{
    public static readonly string SectionName = "FreshMenuStorageOptions";

    public void Configure(FreshMenuStorageOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
