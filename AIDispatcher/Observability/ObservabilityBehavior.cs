using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIDispatcher.Observability;
// <summary>
/// Adds OpenTelemetry tracing and logging around the request pipeline.
/// </summary>
public class ObservabilityBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private static readonly ActivitySource ActivitySource = new("AIDispatcher");
    private readonly ILogger<ObservabilityBehavior<TRequest, TResponse>> _logger;

    public ObservabilityBehavior(ILogger<ObservabilityBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken,
        DispatcherHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;

        using var activity = ActivitySource.StartActivity($"AIDispatcher: {requestName}", ActivityKind.Internal);

        _logger.LogInformation("Handling request {RequestType}", requestName);

        activity?.SetTag("request.type", requestName);
        activity?.SetTag("dispatcher.layer", "AIDispatcher");

        try
        {
            var response = await next();

            _logger.LogInformation("Successfully handled request {RequestType}", requestName);
            activity?.SetTag("request.success", true);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request {RequestType}", requestName);
            activity?.SetTag("request.success", false);
            activity?.SetTag("exception", ex.Message);
            throw;
        }
    }
}