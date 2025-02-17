using Subway.Mvp.Shared;

namespace Subway.Mvp.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
