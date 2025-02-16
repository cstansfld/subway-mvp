using Microsoft.Extensions.Caching.Hybrid;
using Subway.Mvp.Application.Abstractions.Data;
using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Domain.FreshMenuVotes;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Features.FreshMenu.Votes.Create;

internal sealed class VoteForFreshMealCommandHandler(
    HybridCache _cache, IApplicationDbContext _applicationDbContext,
    ILogger<VoteForFreshMealCommandHandler> _logger)
    : ICommandHandler<VoteForFreshMealCommand, VoteForFreshMealResponse>
{
    public async Task<Result<VoteForFreshMealResponse>> Handle(VoteForFreshMealCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            FreshMenuVote voteplaced =
                await _applicationDbContext.VoteForFreshMenuMeal(command.Meal, cancellationToken);

            await _cache.SetAsync(
                    $"votes-by-meal-{voteplaced.Meal}",
                    voteplaced,
                    cancellationToken: cancellationToken);

            return Result.Success(new VoteForFreshMealResponse()
            {
                Meal = voteplaced.Meal,
                VotedFor = voteplaced.VotedFor
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            return Result.Failure<VoteForFreshMealResponse>(FreshMenuVoteErrors.VoteSummaryError);
        }
    }
}
