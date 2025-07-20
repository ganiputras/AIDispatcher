using AIDispatcher.Behaviors;
using AIDispatcher.Notification;
using AIDispatcher.PrePostProcessor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AIDispatcher.Dispatcher;

/// <summary>
/// Extension methods for registering AIDispatcher services into the service collection.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>
    /// Registers all Dispatcher handlers, behaviors, and related services from the specified assemblies.
    /// </summary>
    /// <param name="services">The IServiceCollection to register into.</param>
    /// <param name="assemblies">Assemblies to scan for handlers and processors.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddAIDispatcher(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Core dispatcher
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        // Behaviors (Pipeline)
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(PrePostProcessorBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(TracingBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(DispatcherMetricsBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(RetryBehavior<,>));
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(CircuitBreakerBehavior<,>));

        // Assembly scanning
        foreach (var assembly in assemblies)
        {
            // Request Handlers
            RegisterImplementations(services, assembly, typeof(IDispatcherHandler<,>));
            // Notification Handlers
            RegisterImplementations(services, assembly, typeof(INotificationHandler<>));
            // PreProcessors
            RegisterImplementations(services, assembly, typeof(IRequestPreProcessor<>));
            // PostProcessors
            RegisterImplementations(services, assembly, typeof(IRequestPostProcessor<,>));
        }

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
}