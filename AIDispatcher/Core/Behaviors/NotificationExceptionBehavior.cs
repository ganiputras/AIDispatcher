using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Behavior pipeline untuk mencatat dan melempar ulang exception yang terjadi pada eksekusi handler notifikasi.
/// Setiap error yang tidak tertangani akan dicatat ke log sebagai error sebelum diteruskan ke caller.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class NotificationExceptionBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationExceptionBehavior<TNotification>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="NotificationExceptionBehavior{TNotification}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat error pada handler notifikasi.</param>
    public NotificationExceptionBehavior(ILogger<NotificationExceptionBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani pipeline notifikasi dengan mencatat error bila terjadi exception pada handler notifikasi, lalu melempar ulang exception tersebut.
    /// </summary>
    /// <param name="notification">Notifikasi yang akan diproses.</param>
    /// <param name="handler">Delegate handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler notifikasi.</returns>
    /// <exception cref="Exception">Semua exception yang terjadi pada handler akan dicatat dan dilempar ulang.</exception>
    public async Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        try
        {
            await handler();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing notification {NotificationName}.",
                typeof(TNotification).Name
            );
            throw;
        }
    }
}
