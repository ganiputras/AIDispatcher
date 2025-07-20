namespace AIDispatcher.Notification;

public interface INotificationHandlerWithPriority
{
    int Priority { get; }
}