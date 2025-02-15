using Subway.Mvp.Domain.FreshMenuVotes;

namespace Subway.Mvp.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    Task<List<FreshMenuVote>> GetAllFreshMenuVotes(CancellationToken cancellationToken = default);
    Task<FreshMenuVote> VoteForFreshMenuMeal(string meal, CancellationToken cancellationToken = default);

    IDocumentStoreContainer DocumentStore { get; }
}
