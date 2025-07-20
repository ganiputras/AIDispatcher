using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Options;

namespace AIDispatcher.Behaviors;

/// <summary>
///     Dispatcher behavior to enforce a timeout on request execution.
///     If the request exceeds the configured duration, a TimeoutException is thrown.
/// </summary>
public class TimeoutBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private readonly TimeSpan _timeout;

    public TimeoutBehavior(IOptions<DispatcherOptions> options)
    {
        _timeout = options.Value.DefaultTimeout;
    }

    public async Task<TResponse> HandleAsync(TRequest request,
        CancellationToken cancellationToken,
        Func<CancellationToken, Task<TResponse>> next)
    {
        // Skip timeout logic if set to Infinite
        if (_timeout == Timeout.InfiniteTimeSpan)
            return await next(cancellationToken);

        using var timeoutCts = new CancellationTokenSource(_timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            return await next(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Request of type '{typeof(TRequest).Name}' timed out after {_timeout.TotalMilliseconds}ms.");
        }
    }
}