using AIDispatcher.Notification;

namespace AIDispatcher.Dispatcher;

/// <summary>
///     Global entry point dispatcher untuk root-level call seperti Program.cs atau background service.
///     Secara otomatis membuat service scope.
/// </summary>
public interface IDispatcherRoot
{
    Task<TResult> SendAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : notnull;

    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}