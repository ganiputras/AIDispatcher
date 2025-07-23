using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Behavior pipeline untuk melakukan retry otomatis pada kegagalan handler notifikasi.
/// </summary>
public class RetryNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<RetryNotificationBehavior<TNotification>> _logger;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    public RetryNotificationBehavior(ILogger<RetryNotificationBehavior<TNotification>> logger, int retryCount = 3, int delayMs = 200)
    {
        _logger = logger;
        _retryCount = retryCount;
        _retryDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    /// <summary>
    ///     Menangani pipeline dengan melakukan retry jika terjadi exception pada handler notifikasi.
    /// </summary>
    public async Task Handle(TNotification notification, NotificationHandlerDelegate handler, CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                attempt => TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * attempt),
                (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(exception, "Retry {Attempt} for notification {NotificationName} after {Delay}ms.", attempt, typeof(TNotification).Name, delay.TotalMilliseconds);
                });

        await policy.ExecuteAsync(() => handler());
    }
}