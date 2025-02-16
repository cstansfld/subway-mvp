using Subway.Mvp.Application.Abstractions.Messaging;
using Subway.Mvp.Application.Features.FreshMenu.Meals.GetOne;

namespace Subway.Mvp.Application.Features.FreshMenu.Votes.GetSummary;

public sealed record GetAllVotesQuery() : IQuery<AllVotesSummaryResponse>;
