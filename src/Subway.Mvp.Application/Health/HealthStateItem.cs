namespace Subway.Mvp.Application.Health;

public sealed record HealthStateItem(string? TraceId, string Key, string Detail, int Status, string? Instance)
{
    public string? TraceId { get; init; } = TraceId;
    public string Key { get; init; } = Key;
    public string Detail { get; init; } = Detail;
    public int Status { get; init; } = Status;
    public string? Instance { get; init; } = Instance;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public static HealthStateItem Create(
      string? TraceId, string Key, string Detail, int Status, string? Instance)
    {
        return new HealthStateItem(TraceId, Key, Detail, Status, Instance)
        {
            TraceId = TraceId,
            Key = Key,
            Detail = Detail,
            Status = Status,
            Instance = Instance
        };
    }
}
