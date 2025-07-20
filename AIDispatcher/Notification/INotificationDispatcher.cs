namespace AIDispatcher.Notification;

/// <summary>
/// Interface untuk mendispatch notifikasi ke handler-handler.
/// </summary>
public interface INotificationDispatcher
{
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : notnull;
}
