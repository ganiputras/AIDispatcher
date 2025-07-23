namespace AIDispatcher.Core.Interfaces;

public interface INotificationTimeoutAware
{
    TimeSpan TimeoutPerHandler { get; }
}