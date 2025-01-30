using MediatR;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
