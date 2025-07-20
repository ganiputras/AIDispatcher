namespace AIDispatcher.Notification;

/// <summary>
///     Interface for publishing notifications to one or more handlers.
/// </summary>
public interface INotificationDispatcher
{
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}