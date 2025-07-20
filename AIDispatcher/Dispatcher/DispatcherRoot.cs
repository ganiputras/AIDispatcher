using AIDispatcher.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Dispatcher;

/// <summary>
///     Implementasi IDispatcherRoot yang menggunakan scoped lifetime service.
/// </summary>
internal class DispatcherRoot : IDispatcherRoot
{
    private readonly IServiceProvider _provider;

    public DispatcherRoot(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResult> SendAsync<TRequest, TResult>(TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : notnull
    {
        using var scope = _provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        return await dispatcher.SendAsync<TRequest, TResult>(request, cancellationToken);
    }

    public async Task PublishAsync<TNotification>(TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        using var scope = _provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<INotificationDispatcher>();
        await dispatcher.PublishAsync(notification, cancellationToken);
    }
}