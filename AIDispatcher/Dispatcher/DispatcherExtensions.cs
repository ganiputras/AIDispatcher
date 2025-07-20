using AIDispatcher.Behaviors;
using AIDispatcher.Dispatcher;
using AIDispatcher.Notification;
using AIDispatcher.PrePostProcessor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AIDispatcher;

/// <summary>
/// Registrasi seluruh pipeline AIDispatcher, handler, behavior, dan adapter gaya MediatR.
/// </summary>
/// <param name="services">Container DI ASP.NET Core.</param>
/// <param name="configureBuilder">Opsional, konfigurasi pipeline dengan fluent builder.</param>
/// <param name="configureOptions">Opsional, konfigurasi eksekusi dispatcher seperti paralel, timeout.</param>
/// <param name="assemblies">Assembly tempat mencari handler & behavior. Jika kosong, akan scan semua.</param>
/// <returns>IServiceCollection yang telah diperluas.</returns>
public static class DispatcherExtensions
{
    public static IServiceCollection AddAIDispatcher(
        this IServiceCollection services,
        Action<DispatcherBuilder>? configureBuilder = null,
        Action<DispatcherOptions>? configureOptions = null,
        params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        // Core
        services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();
        services.AddScoped<IDispatcherRoot, DispatcherRoot>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        // Scan & register handler types
        foreach (var assembly in assemblies)
        {
            RegisterImplementations(services, assembly, typeof(IDispatcherHandler<,>));
            RegisterImplementations(services, assembly, typeof(IRequestPreProcessor<>));
            RegisterImplementations(services, assembly, typeof(IRequestPostProcessor<,>));
            RegisterImplementations(services, assembly, typeof(INotificationBehavior<>));
            RegisterNotificationHandlers(services, assembly);
            RegisterRequestHandlers(services, assembly);
        }

        // Apply DispatcherOptions
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        // Build pipeline via fluent builder
        if (configureBuilder != null)
        {
            var builder = new DispatcherBuilder(services);
            configureBuilder(builder);
        }
        else
        {
            // Default behaviors
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


    //Mediatr Style Handlers
    private static void RegisterRequestHandlers(IServiceCollection services, Assembly assembly)
    {
        var allTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var type in allTypes)
        {
            var interfaces = type.GetInterfaces();

            var handlerInterfaces = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .ToList();

            foreach (var handlerInterface in handlerInterfaces)
            {
                var requestType = handlerInterface.GetGenericArguments()[0];
                var responseType = handlerInterface.GetGenericArguments()[1];

                var adapterType = typeof(RequestHandlerAdapter<,>).MakeGenericType(requestType, responseType);
                var dispatcherHandlerType = typeof(IDispatcherHandler<,>).MakeGenericType(requestType, responseType);

                services.AddScoped(handlerInterface, type);

                if (services.All(s => s.ServiceType != dispatcherHandlerType))
                {
                    services.AddScoped(dispatcherHandlerType, adapterType);
                }
            }
        }
    }
}
