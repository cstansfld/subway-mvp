using Subway.Mvp.Shared;

namespace Subway.Mvp.Domain.FreshMenuVotes;

public static class FreshMenuVoteErrors
{
    public static readonly Error VotePlacedError = 
        Error.Problem(
            "FreshMenuVote.VotePlacedError", 
            "Error placing fresh menu vote.");
    public static readonly Error VoteSummaryError =
        Error.Problem(
            "FreshMenuVote.VoteSummaryError", 
            "Error retrieving fresh menu vote summary.");
    public static readonly Error VoteNotAValidFreshMenuMealItemError =
        Error.Validation(
            "FreshMenuVote.VoteNotAValidFreshMenuMealItemError", 
            "FreshMenu Meal is not valid.");
}
