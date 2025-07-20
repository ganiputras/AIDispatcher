namespace AIDispatcher.Notification;

/// <summary>
/// Pipeline behavior untuk notifikasi. Dieksekusi sebelum dan sesudah handler notifikasi.
/// </summary>
public interface INotificationBehavior<TNotification>
    where TNotification : notnull
{
    Task HandleAsync(
        TNotification notification,
        CancellationToken cancellationToken,
        Func<Task> next);
}