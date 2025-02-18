using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Abstractions.Lifetime;
using Subway.Mvp.Application.Features.FreshMenu;

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

    private DocumentStore CreateStore()
    {
        var store = new DocumentStore()
        {
            Urls = [_freshMenuStoreOptions.ServerUrl],
            Database = _freshMenuStoreOptions.DatabaseName,
        };
        store.Initialize();
        store = EnsureDatabaseExists(store, _freshMenuStoreOptions.DatabaseName, _logger);
        store.OnAfterSaveChanges += DocumentStore_OnAfterSaveChanges;
        store.OnBeforeStore += DocumentStore_OnBeforeStore;
        store.OnSessionCreated += DocumentStore_OnSessionCreated;
        store.OnBeforeDelete += DocumentStore_OnBeforeDelete;
        store.OnBeforeQuery += DocumentStore_OnBeforeQuery;
        store.OnSessionDisposing += DocumentStore_OnSessionDisposing;
        try
        {
            store.ExecuteIndexes(FreshMenuIndexes.GetAllFreshMenuIndexes());
        }
        catch (Exception)
        {
            // occurs during docker container tests
        }
        return store;
    }

    private static DocumentStore EnsureDatabaseExists(DocumentStore store, string databaseName,
        ILogger<FreshMenuDocumentStoreContainer> logger)
    {
        try
        {
            DatabaseRecordWithEtag databaseRecordWithEtag = store.Maintenance.Server.Send(new GetDatabaseRecordOperation(databaseName));
            if (databaseRecordWithEtag == null)
            {
                var databaseRecord = new DatabaseRecord(databaseName);
                store.Maintenance.Server.Send(new CreateDatabaseOperation(databaseRecord));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UnexpectedError verifying {Database} in {Ensure}", databaseName, nameof(EnsureDatabaseExists));
        }
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

    #endregion
}
