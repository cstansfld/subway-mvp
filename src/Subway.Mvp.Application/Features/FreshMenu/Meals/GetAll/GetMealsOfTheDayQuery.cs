using Subway.Mvp.Application.Abstractions.Messaging;

namespace Subway.Mvp.Application.Features.FreshMenu.Meals.GetAll;

public sealed record GetMealsOfTheDayQuery() : IQuery<List<MealsOfTheDayResponse>>;
