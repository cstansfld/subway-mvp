using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Subway.Mvp.Application.Abstractions.Behaviors;
using Subway.Mvp.Application.Abstractions.Health;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Application.Health;
using Subway.Mvp.Application.Middleware;

namespace Subway.Mvp.Application;

public static class DependencyInjection
{
    private static HealthState HealthStateProp
    {
        get;
        set;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services = services.GetFreshMenuStorageOptions(configuration);
        services = services.AddMemoryCache();
        services = services.AddRequestResponse();
        services = services.AddHealthState(configuration);

        return services;
    }

    private static IServiceCollection GetFreshMenuStorageOptions(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(FreshMenuStorageSetup.SectionName);
        FreshMenuStorageOptions freshMenuStorageOptions = section.Get<FreshMenuStorageOptions>()!;
        section.Bind(freshMenuStorageOptions);
        services.ConfigureOptions<FreshMenuStorageSetup>();
        services.AddSingleton(freshMenuStorageOptions);
        return services;
    }

    private static IServiceCollection AddRequestResponse(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        return services;
    }

    private static HealthOptions GetHealthOptions(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(HealthOptionsSetup.SectionName);
        HealthOptions healthOptions = section.Get<HealthOptions>()!;
        section.Bind(healthOptions);
        services.ConfigureOptions<HealthOptionsSetup>();
        services.AddSingleton(healthOptions);
        return healthOptions;
    }

    private static IServiceCollection AddHealthState(this IServiceCollection services, IConfiguration configuration)
    {
        HealthStateProp = new HealthState();
        services.AddSingleton<IHealthState>(HealthStateProp);
        services.AddHealthChecks();

        HealthOptions healthOptions = services.GetHealthOptions(configuration);

        // resolve local vs docker
        string? urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        string? endpoint = string.IsNullOrEmpty(urls) ? healthOptions.HealthHost : urls.Split(';')[0];

        services.AddHealthChecksUI(opt =>
        {
            opt.DisableDatabaseMigrations(); // not stored logged
            opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
            opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
            opt.SetApiMaxActiveRequests(3); //api requests concurrency  3 is default  
            opt.AddHealthCheckEndpoint("api-freshmenu", $"{endpoint}/health"); //map health check api    

        }).AddInMemoryStorage();

        return services;
    }

    public static void AddHealthCheckApp(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponseAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            },
            Predicate = _ => true
        });
    }

    private static Task WriteHealthCheckResponseAsync(HttpContext httpContext, HealthReport healthReport)
    {
        httpContext.Response.ContentType = "application/json";

        var dependencyHealthChecks = healthReport.Entries.Select(entry => new
        {
            Name = entry.Key,
            entry.Value.Description,
            Status = entry.Value.Status.ToString(),
            Duration = entry.Value.Duration.ToString("hh\\:mm\\:ss\\.fffffff", CultureInfo.InvariantCulture),
            entry.Value.Data,
            Exception = entry.Value.Exception?.Message
        });

        HealthStatus combinedHealthStatus = HealthStateProp.ErrorItems.CountBy(p => p.Status >= 500).Any() ? HealthStatus.Degraded : healthReport.Status;

        var healthCheckResponse = new
        {
            Status = combinedHealthStatus.ToString(),
            HealthCheckExecutionTime = healthReport.TotalDuration.ToString("hh\\:mm\\:ss\\.fffffff", CultureInfo.InvariantCulture),
            DependencyHealthChecks = dependencyHealthChecks,
            CustomAppState = HealthStateProp
        };

        string responseString = JsonSerializer.Serialize(healthCheckResponse);

        return httpContext.Response.WriteAsync(responseString);
    }

    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();

        return app;
    }

}
