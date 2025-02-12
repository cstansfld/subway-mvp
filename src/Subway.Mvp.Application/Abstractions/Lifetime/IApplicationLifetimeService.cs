namespace Subway.Mvp.Application.Abstractions.Lifetime;

public interface IApplicationLifetimeService
{
    event EventHandler ServiceClosingEvent;

    bool ApplicationStopping { get; }
}
