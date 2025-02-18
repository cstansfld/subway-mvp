using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Abstractions.Messaging;

namespace Subway.Mvp.Tests;

public abstract class BaseFreshMenuFixture : IClassFixture<FreshMenuIntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly IDocumentStoreContainer DocumentStoreContainer;
    protected readonly IApplicationDbContext AppDbContext;
    protected readonly ISender Sender;
    protected readonly FreshMenuIntegrationTestWebAppFactory Factory;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;

    protected BaseFreshMenuFixture(FreshMenuIntegrationTestWebAppFactory factory)
    {
        Factory = factory;
        _scope = Factory.Services.CreateScope();
        DocumentStoreContainer = _scope.ServiceProvider.GetRequiredService<IDocumentStoreContainer>();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
    }

}
