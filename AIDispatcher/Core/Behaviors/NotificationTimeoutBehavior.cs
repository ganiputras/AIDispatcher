using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk membatalkan eksekusi handler notifikasi jika durasi proses melebihi batas waktu (timeout).
/// Mendukung penentuan timeout per instance menggunakan <see cref="ITimeoutAware"/> maupun per class melalui atribut <see cref="WithTimeoutAttribute"/>.
/// Jika tidak ada keduanya, digunakan nilai default timeout (3 detik).
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class NotificationTimeoutBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);
    private readonly ILogger<NotificationTimeoutBehavior<TNotification>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="NotificationTimeoutBehavior{TNotification}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat peristiwa timeout.</param>
    public NotificationTimeoutBehavior(ILogger<NotificationTimeoutBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani eksekusi handler notifikasi dengan menerapkan mekanisme timeout berdasarkan konfigurasi yang ditemukan.
    /// Jika eksekusi handler melebihi batas waktu yang ditentukan, proses akan dibatalkan dan dicatat ke log.
    /// </summary>
    /// <param name="notification">Notifikasi yang akan diproses.</param>
    /// <param name="handler">Delegate handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler notifikasi.</returns>
    /// <exception cref="TimeoutException">Dilempar jika proses handler melebihi batas waktu yang diizinkan.</exception>
    public async Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        TimeSpan timeout;

        // 1. Cek instance (per-object) => ITimeoutAware
        if (notification is ITimeoutAware aware)
        {
            timeout = aware.Timeout;
        }
        // 2. Cek attribute [WithTimeout] di type (per-class)
        else
        {
            var attr = typeof(TNotification).GetCustomAttribute<WithTimeoutAttribute>();
            timeout = attr != null
                ? TimeSpan.FromMilliseconds(attr.TimeoutMilliseconds)
                : DefaultTimeout;
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        var task = handler();
        var completed = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));
        if (completed != task)
        {
            _logger.LogWarning(
                "Notification handler {NotificationName} timed out after {Timeout} ms.",
                typeof(TNotification).Name, timeout.TotalMilliseconds);
            throw new TimeoutException(
                $"Notification {typeof(TNotification).Name} exceeded the timeout of {timeout.TotalMilliseconds} ms.");
        }

        await task;
    }
}
