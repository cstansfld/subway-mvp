using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Subway.Mvp.Application.Abstractions.Lifetime;

namespace Subway.Mvp.Infrastructure.Lifetime;

public sealed class ApplicationLifetimeService(ILogger<ApplicationLifetimeService> _logger /*,IServiceScopeFactory _serviceScopeFactory*/)
    : IApplicationLifetimeService, IHostedService, IDisposable
{
    ~ApplicationLifetimeService()
    {
        Dispose(false);
    }

    public event EventHandler ServiceClosingEvent;

    public bool ApplicationStopping
    {
        get;
        private set;
    }

    // a deterministic timer firing every 5 seconds not after Task.Delay
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application Starting");

        Task.Run(async () =>
        {
            while (!ApplicationStopping)
            {
                try
                {
                    _logger.LogInformation("Application Time Event {AppTime}", DateTimeOffset.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled Exception {AppTime}", DateTimeOffset.UtcNow);
                }

                await _timer.WaitForNextTickAsync(cancellationToken);
            }
        }, cancellationToken);

        _logger.LogInformation("Application Started");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!ApplicationStopping)
        {
            ApplicationStopping = true;

            NotifyAllSubscribersOfAppShuttingdown();
        }

        _logger.LogInformation("Application Stopping {AppStopping}", ApplicationStopping);

        Task completedTask = Task.CompletedTask;

        _logger.LogInformation("Application Stopped");

        return completedTask;
    }

    internal void NotifyAllSubscribersOfAppShuttingdown()
    {
        ServiceClosingEvent?.Invoke(this, new EventArgs());
    }

    #region IDisposable

    private bool disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _timer?.Dispose();

                if (ServiceClosingEvent != null)
                {
                    foreach (Delegate d in ServiceClosingEvent.GetInvocationList())
                    {
                        ServiceClosingEvent -= (EventHandler)d;
                    }
                }
            }

            // Mark the object as disposed
            disposed = true;
        }
    }

    #endregion
}
