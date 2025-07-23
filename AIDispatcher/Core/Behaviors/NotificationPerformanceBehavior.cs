using System.Diagnostics;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Behavior untuk mencatat waktu eksekusi notifikasi dan me-log jika melebihi ambang batas.
/// </summary>
public class NotificationPerformanceBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationPerformanceBehavior<TNotification>> _logger;
    private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);

    public NotificationPerformanceBehavior(ILogger<NotificationPerformanceBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Menangani pipeline notification, mencatat peringatan jika waktu eksekusi handler melebihi threshold.
    /// </summary>
    public async Task Handle(TNotification notification, NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        await handler();
        stopwatch.Stop();

        if (stopwatch.Elapsed > _threshold)
            _logger.LogWarning(
                "Notification handler {NotificationName} execution time exceeded threshold: {ElapsedMilliseconds} ms.",
                typeof(TNotification).Name,
                stopwatch.ElapsedMilliseconds
            );
    }
}