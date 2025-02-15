﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raven.Embedded;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Infrastructure.Caching;
using Subway.Mvp.Infrastructure.Database;
using Subway.Mvp.Infrastructure.FreshMenu;

namespace Subway.Mvp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
       this IServiceCollection services)
    {
        return services.AddCache().FreshApiDocumentStorage();
    }

    private static IServiceCollection FreshApiDocumentStorage(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentStoreContainer, FreshMenuDocumentStoreContainer>();
        services.AddSingleton<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024 * 10;
            options.MaximumKeyLength = 512;
            options.ReportTagMetrics = true;
            options.DefaultEntryOptions = DefaultHybridCacheOptions.DefaultCacheSettings;
        });
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        return services;
    }

    public static void StartEmbeddedFreshApiDatabase(this WebApplication app)
    {
        IServiceProvider serviceProvider = app.Services;
        IOptions<FreshMenuStorageOptions> freshMenuOptions = serviceProvider.GetRequiredService<IOptions<FreshMenuStorageOptions>>();

        try
        {
            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = freshMenuOptions.Value.DataDirectory,
            });
        }
        catch (Exception)
        {
            // embedded server already started
        }

    }
}
