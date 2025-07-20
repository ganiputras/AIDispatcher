using System.Diagnostics.Metrics;
using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Behaviors;

/// <summary>
///     Emits metrics for dispatched requests, compatible with OpenTelemetry Metrics (e.g., Prometheus/Grafana).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class DispatcherMetricsBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private static readonly Meter Meter = new("AIDispatcher.Metrics");
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("dispatcher_requests_total");

    private static readonly Histogram<double> DurationHistogram =
        Meter.CreateHistogram<double>("dispatcher_duration_ms");

    private readonly ILogger<DispatcherMetricsBehavior<TRequest, TResponse>> _logger;

    public DispatcherMetricsBehavior(ILogger<DispatcherMetricsBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken,
        Func<CancellationToken, Task<TResponse>> next)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = DateTime.UtcNow;

        _logger.LogDebug("Metrics started for {RequestType}", requestName);

        try
        {
            var response = await next(cancellationToken);

            RequestCounter.Add(1, KeyValuePair.Create<string, object?>("request_type", requestName));
            DurationHistogram.Record((DateTime.UtcNow - startTime).TotalMilliseconds,
                KeyValuePair.Create<string, object?>("request_type", requestName));

            return response;
        }
        catch (Exception ex)
        {
            RequestCounter.Add(1, new KeyValuePair<string, object?>[]
            {
                new("request_type", requestName),
                new("exception", ex.GetType().Name)
            });

            throw;
        }
    }
}