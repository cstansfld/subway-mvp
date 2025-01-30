using Subway.Mvp.Application.Health;

namespace Subway.Mvp.Application.Abstractions.Health;

public interface IHealthState
{
    List<HealthStateItem> ErrorItems { get; }
    public Dictionary<string, EndpointStatusItem> EndpointStatuses { get; }
    void AddItem(HealthStateItem item);
    int CreateOrUpdateEndpointStatus(string Path, int StatusCode);
}
