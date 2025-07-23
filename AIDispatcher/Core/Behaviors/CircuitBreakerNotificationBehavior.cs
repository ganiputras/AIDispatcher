using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Behavior pipeline untuk circuit breaker pada eksekusi handler notifikasi.
/// </summary>
public class CircuitBreakerNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<CircuitBreakerNotificationBehavior<TNotification>> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public CircuitBreakerNotificationBehavior(ILogger<CircuitBreakerNotificationBehavior<TNotification>> logger, int exceptionsAllowedBeforeBreaking = 2, int durationOfBreakMs = 2000)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromMilliseconds(durationOfBreakMs),
                (ex, breakDelay) =>
                {
                    _logger.LogWarning(ex, "Notification circuit broken for {BreakDelay}ms after exception in {NotificationName}.", breakDelay.TotalMilliseconds, typeof(TNotification).Name);
                },
                () =>
                {
                    _logger.LogInformation("Notification circuit reset for {NotificationName}.", typeof(TNotification).Name);
                });
    }

    /// <summary>
    ///     Menangani pipeline notifikasi dengan circuit breaker, memutus eksekusi jika error terus-menerus.
    /// </summary>
    public async Task Handle(TNotification notification, NotificationHandlerDelegate handler, CancellationToken cancellationToken)
    {
        await _circuitBreakerPolicy.ExecuteAsync(() => handler());
    }
}