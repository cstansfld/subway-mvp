using Subway.Mvp.Domain.FreshMenu;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Domain.FreshMenuVotes;

public sealed class FreshMenuVote
{
    public required string Meal { get; init; }
    public int VotedFor { get; set; }
}
