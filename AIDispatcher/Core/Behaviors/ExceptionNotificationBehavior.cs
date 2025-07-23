using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk menangani exception global yang terjadi pada eksekusi handler notifikasi.
/// Exception yang tidak tertangani akan dicatat ke log sebagai error, kemudian dilempar ulang dalam bentuk <see cref="DispatcherException"/>.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class ExceptionNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<ExceptionNotificationBehavior<TNotification>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="ExceptionNotificationBehavior{TNotification}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat error saat eksekusi handler notifikasi.</param>
    public ExceptionNotificationBehavior(ILogger<ExceptionNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani pipeline notifikasi dengan mencatat dan melempar ulang exception dari handler notifikasi.
    /// Exception yang terjadi akan dicatat ke log, kemudian dilempar ulang dalam bentuk <see cref="DispatcherException"/>.
    /// </summary>
    /// <param name="notification">Notifikasi yang akan diproses.</param>
    /// <param name="next">Delegate handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler notifikasi.</returns>
    /// <exception cref="DispatcherException">Dilempar jika terjadi exception saat eksekusi handler notifikasi.</exception>
    public async Task Handle(
        TNotification notification,
        NotificationHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing notification {NotificationType}.",
                typeof(TNotification).Name
            );
            throw new DispatcherException(
                $"Failed to execute notification {typeof(TNotification).Name}.", ex
            );
        }
    }
}
