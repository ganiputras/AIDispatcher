namespace AIDispatcher.Notification;

/// <summary>
/// Interface untuk menangani notifikasi berbasis event.
/// </summary>
public interface INotificationHandler<in TNotification>
    where TNotification : notnull
{
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}