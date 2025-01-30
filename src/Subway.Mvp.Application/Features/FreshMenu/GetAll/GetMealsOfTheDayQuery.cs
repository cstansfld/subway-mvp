using Subway.Mvp.Application.Abstractions.Messaging;

namespace Subway.Mvp.Application.Features.FreshMenu.GetAll;

public sealed record GetMealsOfTheDayQuery() : IQuery<List<MealsOfTheDayResponse>>;
