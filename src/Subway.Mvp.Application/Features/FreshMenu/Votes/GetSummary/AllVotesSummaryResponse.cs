using System;

namespace Subway.Mvp.Application.Features.FreshMenu.Votes.GetSummary;

public sealed class AllVotesSummaryResponse
{
    public required int Total { get; init; }
    public required List<VoteDetail> Summary { get; init; } = new List<VoteDetail>();

    public sealed class VoteDetail
    {
        private decimal _asPercentOf;

        public required string Meal { get; init; }
        public required int VotedFor { get; init; }
        public required decimal AsPercentOf
        {
            get => _asPercentOf;
            init => _asPercentOf = Math.Round(value, 2);
        }
    }

    public static AllVotesSummaryResponse Create(List<AllVotes.IndexEntry> allVotes)
    {
        int total = allVotes.Sum(x => x.VotedFor);
        return new AllVotesSummaryResponse
        {
            Total = total,
            Summary = allVotes.Select(category =>
                new VoteDetail
                {
                    Meal = category.Meal,
                    VotedFor = category.VotedFor,
                    AsPercentOf = total > 0 ? ((decimal)category.VotedFor / total * 100) : 0
                }).ToList()
        };
    }
}
