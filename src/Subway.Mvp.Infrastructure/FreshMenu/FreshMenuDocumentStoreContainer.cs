﻿using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Abstractions.Lifetime;
using Subway.Mvp.Application.Features.FreshMenu;
using Subway.Mvp.Infrastructure.Lifetime;

namespace Subway.Mvp.Infrastructure.FreshMenu;

public sealed class FreshMenuDocumentStoreContainer : IDocumentStoreContainer, IDisposable
{
    private readonly ILogger<FreshMenuDocumentStoreContainer> _logger;
    private readonly FreshMenuStorageOptions _freshMenuStoreOptions;
    private readonly IApplicationLifetimeService _applicationLifetime;
    private readonly Lazy<IDocumentStore> _lazyStore;

    public FreshMenuDocumentStoreContainer(FreshMenuStorageOptions freshMenuStoreOptions,
        ILogger<FreshMenuDocumentStoreContainer> logger, IApplicationLifetimeService applicationLifetime)
    {
        _logger = logger;
        _freshMenuStoreOptions = freshMenuStoreOptions;
        _applicationLifetime = applicationLifetime;
        _applicationLifetime.ServiceClosingEvent += ApplicationLifetimeService_ServiceClosingEvent;
        _lazyStore = new Lazy<IDocumentStore>(CreateStore);
    }

    public IDocumentStore Store => _lazyStore.Value;

    private IDocumentStore CreateStore()
    {
        IDocumentStore store = Raven.Embedded.EmbeddedServer.Instance.GetDocumentStore(_freshMenuStoreOptions.DatabaseName);
        store.OnAfterSaveChanges += DocumentStore_OnAfterSaveChanges;
        store.OnBeforeStore += DocumentStore_OnBeforeStore;
        store.OnSessionCreated += DocumentStore_OnSessionCreated;
        store.OnBeforeDelete += DocumentStore_OnBeforeDelete;
        store.OnBeforeQuery += DocumentStore_OnBeforeQuery;
        store.OnSessionDisposing += DocumentStore_OnSessionDisposing;
        store.ExecuteIndexes(FreshMenuIndexes.GetAllFreshMenuIndexes());
        return store;
    }

    private void ApplicationLifetimeService_ServiceClosingEvent(object? sender, EventArgs e)
    {
        _logger.LogInformation("Received applicationClosing event.");

        _applicationLifetime.ServiceClosingEvent -= ApplicationLifetimeService_ServiceClosingEvent;
    }

    private void DocumentStore_OnBeforeStore(object? sender, BeforeStoreEventArgs e)
    {
        _logger.LogInformation("{Name} {SessionId} {Database} {DocumentId} {@Entity}",
            "OnBeforeStore", e.Session.Id, e.Session.DatabaseName, e.DocumentId, e.Entity);
    }

    private void DocumentStore_OnAfterSaveChanges(object? sender, AfterSaveChangesEventArgs e)
    {
        _logger.LogInformation("{Name} {SessionId} {Database} {DocumentId} {@Entity}",
            "OnAfterSaveChanges", e.Session.Id, e.Session.DatabaseName, e.DocumentId, e.Entity);
    }

    private void DocumentStore_OnSessionCreated(object? sender, SessionCreatedEventArgs e)
    {
        _logger.LogInformation("{Name} {SessionId} {Database}",
            "OnSessionCreated", e.Session.Id, e.Session.DatabaseName);
    }

    private void DocumentStore_OnBeforeDelete(object? sender, BeforeDeleteEventArgs e)
    {
        _logger.LogInformation("{Name} {SessionId} {Database} {DocumentId} {@Entity}",
            "OnBeforeDelete", e.Session.Id, e.Session.DatabaseName, e.DocumentId, e.Entity);
    }

    private void DocumentStore_OnBeforeQuery(object? sender, BeforeQueryEventArgs e)
    {
        _logger.LogInformation("{Name} {SessionId} {Database} {Query}",
            "OnBeforeQuery", e.Session.Id, e.Session.DatabaseName, e.QueryCustomization.ToString());
    }

    private void DocumentStore_OnSessionDisposing(object? sender, SessionDisposingEventArgs e)
    {
        _logger.LogInformation("{Name} {SessionId} {Database}",
            "OnSessionDisposing", e.Session.Id, e.Session.DatabaseName);
    }

    #region IDisposable

    private bool disposed;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            if (!disposed && Store != null)
            {
                Store.OnBeforeStore -= DocumentStore_OnBeforeStore;
                Store.OnAfterSaveChanges -= DocumentStore_OnAfterSaveChanges;
                Store.OnBeforeStore -= DocumentStore_OnBeforeStore;
                Store.OnSessionCreated -= DocumentStore_OnSessionCreated;
                Store.OnBeforeDelete -= DocumentStore_OnBeforeDelete;
                Store.OnBeforeQuery -= DocumentStore_OnBeforeQuery;
                Store.OnSessionDisposing -= DocumentStore_OnSessionDisposing;

                Store.Dispose();
            }
            disposed = true;
        }
    }

    public async Task SaveChangesAsync(IAsyncDocumentSession session, CancellationToken cancellationToken = default)
    {
        await session.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
