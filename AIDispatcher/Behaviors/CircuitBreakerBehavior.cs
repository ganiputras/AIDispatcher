using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AIDispatcher.Behaviors;

/// <summary>
/// Behavior untuk memblokir request jika jumlah error melebihi ambang batas.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class CircuitBreakerBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private static readonly ConcurrentDictionary<string, int> FailureCounts = new();
    private readonly int _failureThreshold;
    private readonly ILogger<CircuitBreakerBehavior<TRequest, TResponse>> _logger;

    public CircuitBreakerBehavior(
        ILogger<CircuitBreakerBehavior<TRequest, TResponse>> logger,
        int failureThreshold = 3)
    {
        _logger = logger;
        _failureThreshold = failureThreshold;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken,
        DispatcherHandlerDelegate<TResponse> next)
    {
        var key = typeof(TRequest).FullName ?? typeof(TRequest).Name;

        if (FailureCounts.TryGetValue(key, out var count) && count >= _failureThreshold)
        {
            _logger.LogError("Circuit breaker open for {RequestType}", key);
            throw new InvalidOperationException($"Circuit breaker open for {key}");
        }

        try
        {
            var response = await next();
            FailureCounts.TryRemove(key, out _);
            return response;
        }
        catch (Exception ex)
        {
            FailureCounts.AddOrUpdate(key, 1, (_, current) => current + 1);
            _logger.LogWarning(ex, "Failure #{Count} for {RequestType}", FailureCounts[key], key);
            throw;
        }
    }
}

//public static IServiceCollection AddCoreBehaviors(this IServiceCollection services)
//{
//    services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(ValidationBehavior<,>));
//    services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(RetryBehavior<,>));
//    services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(CircuitBreakerBehavior<,>));
//    return services;
//}