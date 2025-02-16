using Subway.Mvp.Application.Abstractions.Messaging;

namespace Subway.Mvp.Application.Features.FreshMenu.Votes.Create;

public sealed class VoteForFreshMealCommand : ICommand<VoteForFreshMealResponse>
{
    public string Meal { get; set; }
}
