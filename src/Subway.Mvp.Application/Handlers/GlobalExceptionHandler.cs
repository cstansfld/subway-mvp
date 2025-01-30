using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

using Subway.Mvp.Application.Abstractions.Health;

namespace Subway.Mvp.Application.Handlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger, IWebHostEnvironment environment, IHealthState healthState) : IExceptionHandler
{
    private const string UnhandledError = "UnhandledError";
    private const string DataTracker = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string traceId = GetTraceId(httpContext);
        int statusCode = StatusCodes.Status500InternalServerError;
        string srvcName = environment.ApplicationName;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = $"{UnhandledError} {srvcName}",
            Type = $"{DataTracker}",
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        _logger.LogError(exception, "Unhandled exception occurred: {@ProblemDetails} ", problemDetails);

        healthState.AddItem(Health.HealthStateItem.Create(traceId, UnhandledError, exception.Message, statusCode, httpContext.Request.Path.ToString()));

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static string GetTraceId(HttpContext httpContext)
    {
        return httpContext.Features.Get<IHttpActivityFeature>()?.Activity.TraceId.ToString() ?? "No TraceId";
    }
}
