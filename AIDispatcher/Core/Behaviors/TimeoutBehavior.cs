using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk membatalkan permintaan void jika melebihi waktu tertentu (dari ITimeoutAware).
/// </summary>
public class TimeoutBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<TimeoutBehavior<TRequest>> _logger;

    public TimeoutBehavior(ILogger<TimeoutBehavior<TRequest>> logger)
        => _logger = logger;

    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        if (request is not ITimeoutAware timeoutAware)
        {
            await next();
            return;
        }

        using var timeoutCts = new CancellationTokenSource(timeoutAware.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await next().WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Request {RequestName} timed out after {TimeoutSeconds} seconds.",
                typeof(TRequest).Name,
                timeoutAware.Timeout.TotalSeconds
            );
            throw new TimeoutException(
                $"Request {typeof(TRequest).Name} exceeded the timeout of {timeoutAware.Timeout.TotalSeconds} seconds.");
        }
    }
}



/// <summary>
///     Pipeline behavior untuk membatalkan permintaan dengan hasil jika melebihi waktu tertentu (dari ITimeoutAware).
/// </summary>
public class TimeoutBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TimeoutBehavior<TRequest, TResponse>> _logger;

    public TimeoutBehavior(ILogger<TimeoutBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITimeoutAware timeoutAware)
            return await next();

        using var timeoutCts = new CancellationTokenSource(timeoutAware.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            return await next().WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Request {RequestName} timed out after {TimeoutSeconds} seconds.",
                typeof(TRequest).Name,
                timeoutAware.Timeout.TotalSeconds
            );
            throw new TimeoutException(
                $"Request {typeof(TRequest).Name} exceeded the timeout of {timeoutAware.Timeout.TotalSeconds} seconds.");
        }
    }
}