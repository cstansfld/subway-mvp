using System.Collections.Concurrent;
using Subway.Mvp.Application.Abstractions.Health;

namespace Subway.Mvp.Application.Health;

internal sealed class HealthState : IHealthState, IDisposable
{
    private readonly HealthQueue<HealthStateItem> _queue = new(5, Popped);
    private readonly ConcurrentDictionary<string, EndpointStatusItem> _endpointStatuses = new(StringComparer.OrdinalIgnoreCase);

    public List<HealthStateItem> ErrorItems => _queue.GetAll().ToList() ?? [];
    public Dictionary<string, EndpointStatusItem> EndpointStatuses => _endpointStatuses.ToDictionary();

    public void AddItem(HealthStateItem item)
    {
        _queue.Push(item);
    }

    public int CreateOrUpdateEndpointStatus(string Path, int StatusCode)
    {
        EndpointStatusItem createdOrUpdated = _endpointStatuses.AddOrUpdate(Path, new EndpointStatusItem(StatusCode), (k, v) => v.Update(v.Count));
        return createdOrUpdated.StatusCode;
    }

    private static void Popped(HealthStateItem item)
    {
        // Method intentionally left empty.
    }

    public void Dispose()
    {
        _queue.Dispose();
    }
}
