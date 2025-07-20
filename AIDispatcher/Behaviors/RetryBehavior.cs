using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Behaviors;

/// <summary>
///  Behavior untuk mengulangi eksekusi handler saat terjadi exception.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class RetryBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;
    private readonly int _maxRetries;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger, int maxRetries = 3)
    {
        _logger = logger;
        _maxRetries = maxRetries;
    }

    public async Task<TResponse> HandleAsync(TRequest request,
        CancellationToken cancellationToken,
        Func<CancellationToken, Task<TResponse>> next)
    {
        for (var attempt = 1; ; attempt++)
            try
            {
                return await next(cancellationToken);
            }
            catch (Exception ex) when (attempt <= _maxRetries)
            {
                _logger.LogWarning(ex, "Retry {Attempt} for {RequestType}", attempt, typeof(TRequest).Name);
                await Task.Delay(100 * attempt, cancellationToken);
            }
    }
}