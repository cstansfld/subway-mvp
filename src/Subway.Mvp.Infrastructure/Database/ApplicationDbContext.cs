using Microsoft.EntityFrameworkCore;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Domain.FreshMenuVotes;
using Subway.Mvp.Infrastructure.FreshMenu;

namespace Subway.Mvp.Infrastructure.Database;

public sealed class ApplicationDbContext(IDocumentStoreContainer documentStoreContainer/*, IPublisher publisher*/) : IApplicationDbContext
{
    private const string VotesKey = "FreshMenuVotes";

    public async Task<List<FreshMenuVote>> GetAllFreshMenuVotes(CancellationToken cancellationToken = default)
    {
        using Raven.Client.Documents.Session.IAsyncDocumentSession session = DocumentStore.Store.OpenAsyncSession();
        return await session.Query<FreshMenuVote>().ToListAsync(cancellationToken);
    }

    public async Task<FreshMenuVote> VoteForFreshMenuMeal(string meal, CancellationToken cancellationToken = default)
    {
        using Raven.Client.Documents.Session.IAsyncDocumentSession session = DocumentStore.Store.OpenAsyncSession();
        FreshMenuVote vote = await
            session.LoadAsync<FreshMenuVote>($"{VotesKey}/{meal}", cancellationToken);
        vote.VotedFor++;
        await session.SaveChangesAsync(cancellationToken);
        return vote;
    }

    public IDocumentStoreContainer DocumentStore => documentStoreContainer;
}
