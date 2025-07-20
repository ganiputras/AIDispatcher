using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Notification;

/// <summary>
///     Dispatcher untuk menyebarkan notifikasi ke semua handler yang terdaftar.
/// </summary>
public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TNotification>(TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

        foreach (var handler in handlers) await handler.HandleAsync(notification, cancellationToken);
    }
}