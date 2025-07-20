using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Notification;

public static class NotificationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationDispatcher(this IServiceCollection services)
    {
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        return services;
    }
}