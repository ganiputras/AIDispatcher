using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk mencatat aktivitas eksekusi handler notifikasi.
/// Mencatat waktu mulai dan selesai proses setiap handler notifikasi ke dalam log.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class LoggingNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<LoggingNotificationBehavior<TNotification>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="LoggingNotificationBehavior{TNotification}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat aktivitas notifikasi.</param>
    public LoggingNotificationBehavior(ILogger<LoggingNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani pipeline notifikasi dengan mencatat waktu mulai dan selesai eksekusi handler notifikasi.
    /// </summary>
    /// <param name="notification">Notifikasi yang akan diproses.</param>
    /// <param name="handler">Delegate handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler notifikasi.</returns>
    public async Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        var notificationName = typeof(TNotification).Name;
        _logger.LogInformation("Handling notification {NotificationName} started.", notificationName);

        await handler();

        _logger.LogInformation("Handling notification {NotificationName} completed.", notificationName);
    }
}