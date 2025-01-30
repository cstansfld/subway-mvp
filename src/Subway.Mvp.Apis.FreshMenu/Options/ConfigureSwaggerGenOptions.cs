using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Subway.Mvp.Apis.FreshMenu.Options;

/// <summary>
/// configure api versioning options
/// </summary>
/// <param name="_provider"></param>
public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider _provider) : IConfigureNamedOptions<SwaggerGenOptions>
{
    /// <summary>
    /// override nullable name configuration
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    /// <summary>
    /// api version configuration
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"FreshMenu.API v{description}",
                Version = description.ApiVersion.ToString()
            };

            options.SwaggerDoc(description.GroupName, openApiInfo);
        }
    }
}
