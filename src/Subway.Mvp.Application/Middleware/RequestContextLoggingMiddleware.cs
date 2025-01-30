using Subway.Mvp.Application.Abstractions.Health;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Subway.Mvp.Application.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next, IHealthState healthState)
{
    private const string CorrelationIdHeaderName = "Correlation-Id";

    public Task Invoke(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            Task invoke = next.Invoke(context);
            healthState.CreateOrUpdateEndpointStatus(context.Request.Path, context.Response.StatusCode);
            return invoke;
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
