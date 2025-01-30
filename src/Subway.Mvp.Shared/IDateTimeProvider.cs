namespace Subway.Mvp.Shared;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
