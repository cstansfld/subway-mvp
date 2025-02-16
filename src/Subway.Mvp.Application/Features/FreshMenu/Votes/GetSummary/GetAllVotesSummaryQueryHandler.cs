using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Application.Features.FreshMenu.Votes.Create;
using Subway.Mvp.Domain.FreshMenuVotes;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Features.FreshMenu.Votes.GetSummary;

internal sealed class GetAllVotesSummaryQueryHandler(IApplicationDbContext _applicationDbContext,
    ILogger<VoteForFreshMealCommandHandler> _logger)
    : IQueryHandler<GetAllVotesQuery, AllVotesSummaryResponse>
{
    public async Task<Result<AllVotesSummaryResponse>> Handle(GetAllVotesQuery _, CancellationToken cancellationToken)
    {
        try
        {
            List<AllVotes.IndexEntry> allVotes =
                await _applicationDbContext.GetAllFreshMenuVotes(cancellationToken);

            return AllVotesSummaryResponse.Create(allVotes);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            return Result.Failure<AllVotesSummaryResponse>(FreshMenuVoteErrors.VotePlacedError);
        }

    }
}
