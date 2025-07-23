using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Behavior untuk mencatat dan melempar ulang exception dari handler notifikasi.
/// </summary>
public class NotificationExceptionBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationExceptionBehavior<TNotification>> _logger;

    public NotificationExceptionBehavior(ILogger<NotificationExceptionBehavior<TNotification>> logger)
        => _logger = logger;

    /// <summary>
    ///     Menangani pipeline notifikasi, mencatat error bila terjadi exception pada handler notifikasi.
    /// </summary>
    public async Task Handle(TNotification notification, NotificationHandlerDelegate handler, CancellationToken cancellationToken)
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