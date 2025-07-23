using AIDispatcher.Core;
using AIDispatcher.Core.Behaviors;
using AIDispatcher.Core.Interfaces;

namespace AIDispatcher;

using AIDispatcher.Core.Commons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

/// <summary>
///     Ekstensi untuk registrasi AIDispatcher ke Dependency Injection.
///     Cukup satu baris untuk seluruh pipeline dan handler.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registrasi handler dan pipeline inti (core/minimal) untuk AIDispatcher.
    ///     Direkomendasikan untuk semua aplikasi (WAJIB).
    /// </summary>
    /// <param name="services">Service collection aplikasi</param>
    /// <param name="configure">Konfigurasi opsi AIDispatcher (opsional)</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddAIDispatcherCore(
        this IServiceCollection services,
        Action<DispatcherOptions> configure = null)
    {
        // Otomatis register semua handler di seluruh assembly aplikasi
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .ToArray();

        RegisterGenericHandlers(services, assemblies, typeof(IRequestHandler<>));
        RegisterGenericHandlers(services, assemblies, typeof(IRequestHandler<,>));
        RegisterGenericHandlers(services, assemblies, typeof(INotificationHandler<>));

        // Registrasi pipeline utama (WAJIB), urutan penting:
        // Semua pipeline request/command/query
        services.AddTransient(typeof(IPipelineBehavior<>), typeof(ExceptionBehavior<>));      // Global error handling (paling luar)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));        // Logging pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<>), typeof(TimeoutBehavior<>));        // Timeout
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeoutBehavior<,>));

        // Pipeline notification (WAJIB): ExceptionBehavior harus didaftarkan terakhir (paling luar)
        services.AddTransient(typeof(INotificationPipelineBehavior<>), typeof(NotificationTimeoutBehavior<>));      // Timeout notification
        services.AddTransient(typeof(INotificationPipelineBehavior<>), typeof(NotificationExceptionBehavior<>));    // Global error handling notification

        // Konfigurasi opsi custom, jika ada (opsional)
        if (configure != null)
        {
            var options = new DispatcherOptions();
            configure(options);
            services.AddSingleton(options);
        }

        // Registrasi dispatcher utama
        services.AddTransient<IDispatcher, Dispatcher>();
        return services;
    }

    /// <summary>
    ///     Registrasi pipeline lanjutan/optional untuk AIDispatcher (performance, retry, circuit breaker, dsb).
    ///     Aktifkan sesuai kebutuhan di project implementasi.
    /// </summary>
    /// <param name="services">Service collection aplikasi</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddAIDispatcherAdvanced(this IServiceCollection services)
    {
        // Pipeline request advanced/optional
        services.AddTransient(typeof(IPipelineBehavior<>), typeof(PerformanceBehavior<>));             // Performance monitor (void)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));           // Performance monitor (dengan response)
        services.AddTransient(typeof(IPipelineBehavior<>), typeof(RetryBehavior<>));                   // Retry otomatis saat error (void)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));                 // Retry otomatis saat error (dengan response)
        services.AddTransient(typeof(IPipelineBehavior<>), typeof(CircuitBreakerBehavior<>));          // Circuit breaker proteksi overload  (void)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CircuitBreakerBehavior<,>));        // Circuit breaker proteksi overload  (dengan response)

        // Pipeline notification advanced/optional
        services.AddTransient(typeof(INotificationPipelineBehavior<>), typeof(NotificationPerformanceBehavior<>)); // Performance monitor notification
        services.AddTransient(typeof(INotificationPipelineBehavior<>), typeof(RetryNotificationBehavior<>));       // Retry notification
        services.AddTransient(typeof(INotificationPipelineBehavior<>), typeof(CircuitBreakerNotificationBehavior<>)); // Circuit breaker notification
        services.AddTransient(typeof(INotificationPipelineBehavior<>), typeof(LoggingNotificationBehavior<>));        // Logging notification

        return services;
    }

    /// <summary>
    ///     Mendaftarkan semua handler generic dari seluruh assembly aplikasi secara otomatis.
    /// </summary>
    /// <param name="services">Service collection aplikasi</param>
    /// <param name="assemblies">Daftar assembly</param>
    /// <param name="openGenericType">Tipe interface handler yang ingin diregistrasi</param>
    private static void RegisterGenericHandlers(IServiceCollection services, Assembly[] assemblies, Type openGenericType)
    {
        // Temukan semua class handler yang mengimplementasi interface generic, lalu register ke DI
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                .Select(i => new { Handler = t, Interface = i }))
            .ToList();

        foreach (var handler in handlerTypes)
        {
            // TryAddTransient = hanya tambah jika belum ada, aman untuk handler (anti double registration)
            services.TryAddTransient(handler.Interface, handler.Handler);
        }
    }
}
