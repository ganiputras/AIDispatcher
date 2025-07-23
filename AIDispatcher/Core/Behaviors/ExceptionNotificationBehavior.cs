using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk menangani exception global pada notifikasi.
/// </summary>
public class ExceptionNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<ExceptionNotificationBehavior<TNotification>> _logger;

    public ExceptionNotificationBehavior(ILogger<ExceptionNotificationBehavior<TNotification>> logger)
        => _logger = logger;

    /// <summary>
    ///     Menangani pipeline notifikasi, mencatat dan melempar ulang exception dari handler notifikasi.
    /// </summary>
    public async Task Handle(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
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