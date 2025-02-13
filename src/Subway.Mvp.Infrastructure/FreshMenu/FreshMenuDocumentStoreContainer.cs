using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Embedded;
using Subway.Mvp.Application.Abstractions;
using Subway.Mvp.Application.Features.FreshMenu;

namespace Subway.Mvp.Infrastructure.FreshMenu;

public sealed class FreshMenuDocumentStoreContainer : IDocumentStoreContainer, IDisposable
{
    private readonly ILogger<FreshMenuDocumentStoreContainer> _logger;
    private readonly FreshMenuStorageOptions _freshMenuStoreOptions;

    public FreshMenuDocumentStoreContainer(FreshMenuStorageOptions freshMenuStoreOptions,
        ILogger<FreshMenuDocumentStoreContainer> logger)
    {
        _logger = logger;
        _freshMenuStoreOptions = freshMenuStoreOptions;
        LazyStore = new(() =>
        {
            IDocumentStore store = EmbeddedServer.Instance.GetDocumentStore(_freshMenuStoreOptions.DatabaseName);
            store.OnAfterSaveChanges += DocumentStore_OnAfterSaveChanges;
            store.OnBeforeStore += DocumentStore_OnBeforeStore;
            store.OnSessionCreated += DocumentStore_OnSessionCreated;
            store.OnBeforeDelete += DocumentStore_OnBeforeDelete;
            store.OnBeforeQuery += DocumentStore_OnBeforeQuery;
            store.OnSessionDisposing += DocumentStore_OnSessionDisposing;
            new FreshMenuIndexes.MealByDayOfTheWeek().Execute(store);
            new FreshMenuIndexes.DayOfTheWeekByMeal().Execute(store);
            return store;
        });
    }

    private Lazy<IDocumentStore> LazyStore { get; }

    public IDocumentStore Store => LazyStore.Value;

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
        _logger.LogInformation("{Name} {SessionId} {Database} {@Query}",
            "OnBeforeQuery", e.Session.Id, e.Session.DatabaseName, e.QueryCustomization.QueryOperation.IndexQuery);
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
