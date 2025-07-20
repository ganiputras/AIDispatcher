using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AIDispatcher.Notification;

/// <summary>
/// Implementasi default NotificationDispatcher.
/// Mendukung handler paralel, prioritas, dan pipeline.
/// </summary>
internal class NotificationDispatcher : INotificationDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DispatcherOptions _options;

    public NotificationDispatcher(IServiceProvider serviceProvider, IOptions<DispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : notnull
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var handlers = services.GetServices<INotificationHandler<TNotification>>().ToList();
        var behaviors = services.GetServices<INotificationBehavior<TNotification>>().ToList();

        if (!handlers.Any())
            return;

        if (_options.NotificationHandlerPriorityEnabled)
            handlers = ApplyPriority(handlers);

        if (_options.ParallelNotificationHandlers)
        {
            var tasks = handlers.Select(handler =>
                ExecutePipeline(notification, handler, behaviors, cancellationToken));
            await Task.WhenAll(tasks);
        }
        else
        {
            foreach (var handler in handlers)
                await ExecutePipeline(notification, handler, behaviors, cancellationToken);
        }
    }

    private static List<INotificationHandler<TNotification>> ApplyPriority<TNotification>(
        List<INotificationHandler<TNotification>> handlers)
        where TNotification : notnull
    {
        return handlers
            .OrderByDescending(h => (h as INotificationHandlerWithPriority)?.Priority ?? 0)
            .ToList();
    }

    private static Task ExecutePipeline<TNotification>(
        TNotification notification,
        INotificationHandler<TNotification> handler,
        List<INotificationBehavior<TNotification>> behaviors,
        CancellationToken cancellationToken)
        where TNotification : notnull
    {
        return NotificationPipelineExecutor.ExecuteAsync(
            notification,
            behaviors,
            () => handler.Handle(notification, cancellationToken),
            cancellationToken
        );
    }
}
