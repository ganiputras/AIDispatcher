using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Behavior pipeline untuk memonitor dan mencatat durasi eksekusi handler notifikasi.
/// Akan mencatat warning ke log jika waktu eksekusi melebihi ambang batas yang telah ditentukan (default: 500 ms).
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class NotificationPerformanceBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationPerformanceBehavior<TNotification>> _logger;
    private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Membuat instance baru dari <see cref="NotificationPerformanceBehavior{TNotification}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat warning performa eksekusi notifikasi.</param>
    public NotificationPerformanceBehavior(ILogger<NotificationPerformanceBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani pipeline handler notifikasi dengan mengukur durasi eksekusi dan mencatat peringatan di log jika melebihi threshold.
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
