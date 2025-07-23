using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk melakukan retry otomatis pada kegagalan eksekusi handler request.
/// </summary>
public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger, int retryCount = 3, int delayMs = 200)
    {
        _logger = logger;
        _retryCount = retryCount;
        _retryDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    /// <summary>
    ///     Menangani pipeline dengan melakukan retry sesuai policy jika terjadi exception.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                attempt => TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * attempt),
                (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(exception, "Retry {Attempt} for {RequestName} after {Delay}ms.", attempt, typeof(TRequest).Name, delay.TotalMilliseconds);
                });

        return await policy.ExecuteAsync(() => next());
    }
}



/// <summary>
///     Pipeline behavior untuk melakukan retry otomatis pada handler request tanpa hasil (void).
/// </summary>
public class RetryBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<RetryBehavior<TRequest>> _logger;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    public RetryBehavior(ILogger<RetryBehavior<TRequest>> logger, int retryCount = 3, int delayMs = 200)
    {
        _logger = logger;
        _retryCount = retryCount;
        _retryDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    /// <inheritdoc />
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                attempt => TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * attempt),
                (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(exception, "Retry {Attempt} for {RequestName} after {Delay}ms.", attempt, typeof(TRequest).Name, delay.TotalMilliseconds);
                });

        await policy.ExecuteAsync(() => next());
    }
}
