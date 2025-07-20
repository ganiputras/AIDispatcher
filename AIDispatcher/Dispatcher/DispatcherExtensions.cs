using AIDispatcher.Behaviors;
using AIDispatcher.Notification;
using AIDispatcher.PrePostProcessor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AIDispatcher.Dispatcher;


public static class DispatcherExtensions
{

    public static IServiceCollection AddAIDispatcher(
        this IServiceCollection services,
        Action<DispatcherOptions> configure,
        params Assembly[] assemblies)
    {
        services.AddAIDispatcher(assemblies);
        services.Configure(configure);
        return services;
    }
    public static IServiceCollection AddAIDispatcher(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Core dispatcher
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IDispatcherRoot, DispatcherRoot>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        // Pipeline Behaviors
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(PrePostProcessorBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(TracingBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(DispatcherMetricsBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(RetryBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(CircuitBreakerBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(TimeoutBehavior<,>));


        services.AddScoped(typeof(INotificationBehavior<>), typeof(LoggingNotificationBehavior<>));
        services.AddScoped(typeof(INotificationBehavior<>), typeof(RetryNotificationBehavior<>));
        services.AddScoped(typeof(INotificationBehavior<>), typeof(MetricsNotificationBehavior<>));

        // Scan assemblies
        foreach (var assembly in assemblies)
        {
            RegisterImplementations(services, assembly, typeof(IDispatcherHandler<,>));
            RegisterImplementations(services, assembly, typeof(IRequestPreProcessor<>));
            RegisterImplementations(services, assembly, typeof(IRequestPostProcessor<,>));
            RegisterImplementations(services, assembly, typeof(INotificationBehavior<>));

            // Custom registration for Notification Handlers with priority support
            RegisterNotificationHandlers(services, assembly);
        }

        services.Configure<DispatcherOptions>(options =>
        {
            options.DefaultTimeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

    private static void RegisterImplementations(IServiceCollection services, Assembly assembly, Type interfaceType)
    {
        var types = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
            .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == interfaceType)
            .ToList();

        foreach (var type in types)
        {
            services.AddScoped(type.Interface, type.Type);
        }
    }

    private static void RegisterNotificationHandlers(IServiceCollection services, Assembly assembly)
    {
        var allTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var type in allTypes)
        {
            var interfaces = type.GetInterfaces();

            var handlerInterfaces = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .ToList();

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }

            if (typeof(INotificationHandlerWithPriority).IsAssignableFrom(type))
            {
                services.AddScoped(typeof(INotificationHandlerWithPriority), type);
            }
        }
    }
}