namespace Subway.Mvp.Application.Health;

public sealed class EndpointStatusItem : IEqualityComparer<EndpointStatusItem>
{
    public EndpointStatusItem(int statusCode)
    {
        StatusCode = statusCode;
        Count = 1;
    }

    public int StatusCode { get; private set; }
    public int Count { get; private set; }

    public EndpointStatusItem Update(int count)
    {
        if (count == Count)
        {
            Count += 1;
        }
        return this;
    }

    public bool Equals(EndpointStatusItem? x, EndpointStatusItem? y)
    {
        return x != null && y != null && x.StatusCode == y.StatusCode;
    }

    public int GetHashCode(EndpointStatusItem? endpointStatusItem) =>
        endpointStatusItem?.StatusCode.GetHashCode() ?? throw new ArgumentNullException(nameof(endpointStatusItem));
}
