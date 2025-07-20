using System.Diagnostics;
using AIDispatcher.Dispatcher;
using AIDispatcher.Observability;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace AIDispatcher.Behaviors;

/// <summary>
///     Adds distributed tracing support to the dispatcher pipeline using ActivitySource.
///     Compatible with OpenTelemetry, Jaeger, Zipkin, or other tracing systems.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class TracingBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private static readonly ActivitySource ActivitySource = DispatcherActivitySource.Instance;
    private readonly ILogger<TracingBehavior<TRequest, TResponse>> _logger;

    public TracingBehavior(ILogger<TracingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken,
        Func<CancellationToken, Task<TResponse>> next)
    {
        var requestName = typeof(TRequest).Name;

        using var activity = ActivitySource.StartActivity($"Dispatcher.{requestName}");

        if (activity != null)
        {
            activity.SetTag("dispatcher.request_type", requestName);
            activity.SetTag("dispatcher.request", request.ToString());
        }

        _logger.LogDebug("Tracing started for {RequestType}", requestName);

        try
        {
            var response = await next(cancellationToken);

            if (activity != null) activity.SetTag("dispatcher.success", true);

            return response;
        }
        catch (Exception ex)
        {
            if (activity != null)
            {
                activity.SetTag("dispatcher.success", false);
                activity.SetTag("dispatcher.exception", ex.Message);
                activity.RecordException(ex);
            }

            _logger.LogError(ex, "Tracing failure for {RequestType}", requestName);
            throw;
        }
    }
}