namespace AIDispatcher.Notification;

/// <summary>
///     Interface for handling notifications (events).
/// </summary>
/// <typeparam name="TNotification">The type of notification.</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}