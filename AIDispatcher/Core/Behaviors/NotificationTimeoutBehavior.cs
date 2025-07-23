using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk membatalkan handler notifikasi jika melebihi batas waktu.
///     Mendukung [WithTimeout] attribute dan ITimeoutAware per-instance.
/// </summary>
public class NotificationTimeoutBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<NotificationTimeoutBehavior<TNotification>> _logger;
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);

    public NotificationTimeoutBehavior(ILogger<NotificationTimeoutBehavior<TNotification>> logger)
        => _logger = logger;

    public async Task Handle(TNotification notification, NotificationHandlerDelegate handler, CancellationToken cancellationToken)
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
