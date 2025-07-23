using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk menerapkan pola circuit breaker pada eksekusi handler notifikasi.
/// Jika terjadi error secara berturut-turut, eksekusi handler akan diputus (break) sementara waktu sesuai pengaturan.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class CircuitBreakerNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly ILogger<CircuitBreakerNotificationBehavior<TNotification>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="CircuitBreakerNotificationBehavior{TNotification}"/> dengan konfigurasi circuit breaker.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat status dan error circuit breaker.</param>
    /// <param name="exceptionsAllowedBeforeBreaking">Jumlah maksimum exception yang diperbolehkan sebelum circuit breaker aktif (default: 2).</param>
    /// <param name="durationOfBreakMs">Durasi waktu (dalam ms) circuit breaker akan aktif sebelum reset (default: 2000 ms).</param>
    public CircuitBreakerNotificationBehavior(
        ILogger<CircuitBreakerNotificationBehavior<TNotification>> logger,
        int exceptionsAllowedBeforeBreaking = 2,
        int durationOfBreakMs = 2000)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromMilliseconds(durationOfBreakMs),
                (ex, breakDelay) =>
                {
                    _logger.LogWarning(ex,
                        "Notification circuit broken for {BreakDelay}ms after exception in {NotificationName}.",
                        breakDelay.TotalMilliseconds, typeof(TNotification).Name);
                },
                () =>
                {
                    _logger.LogInformation("Notification circuit reset for {NotificationName}.",
                        typeof(TNotification).Name);
                });
    }

    /// <summary>
    /// Menangani pipeline notifikasi dengan menerapkan mekanisme circuit breaker.
    /// Jika terjadi exception secara berturut-turut melebihi batas yang ditentukan, eksekusi handler akan diputus sementara.
    /// </summary>
    /// <param name="notification">Notifikasi yang akan diproses.</param>
    /// <param name="handler">Delegate handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler notifikasi.</returns>
    /// <exception cref="BrokenCircuitException">Dilempar jika circuit breaker sedang aktif (open state).</exception>
    public async Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        await _circuitBreakerPolicy.ExecuteAsync(() => handler());
    }
}
