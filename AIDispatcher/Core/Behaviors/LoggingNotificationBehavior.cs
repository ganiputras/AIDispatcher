using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline untuk mencatat aktivitas notifikasi per handler.
/// </summary>
public class LoggingNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<LoggingNotificationBehavior<TNotification>> _logger;

    public LoggingNotificationBehavior(ILogger<LoggingNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Menangani pipeline notification, mencatat waktu mulai dan selesai eksekusi handler notifikasi.
    /// </summary>
    public async Task Handle(TNotification notification, NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        var notificationName = typeof(TNotification).Name;
        _logger.LogInformation("Handling notification {NotificationName} started.", notificationName);

        await handler();

        _logger.LogInformation("Handling notification {NotificationName} completed.", notificationName);
    }
}