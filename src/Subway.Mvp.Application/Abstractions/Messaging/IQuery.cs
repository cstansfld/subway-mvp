using MediatR;
using Subway.Mvp.Shared;

namespace Subway.Mvp.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
