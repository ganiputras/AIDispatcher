using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Behavior pipeline untuk melakukan retry otomatis jika terjadi kegagalan (exception) pada handler notifikasi.
/// Menggunakan strategi retry dengan delay bertahap untuk meningkatkan keandalan eksekusi notifikasi.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
public class RetryNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<RetryNotificationBehavior<TNotification>> _logger;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    /// <summary>
    /// Membuat instance baru dari <see cref="RetryNotificationBehavior{TNotification}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat proses retry.</param>
    /// <param name="retryCount">Jumlah maksimum percobaan retry saat terjadi exception (default 3 kali).</param>
    /// <param name="delayMs">Delay (dalam milidetik) antara percobaan retry (default 200 ms).</param>
    public RetryNotificationBehavior(ILogger<RetryNotificationBehavior<TNotification>> logger, int retryCount = 3,
        int delayMs = 200)
    {
        _logger = logger;
        _retryCount = retryCount;
        _retryDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    /// <summary>
    /// Menangani pipeline handler notifikasi dengan mekanisme retry otomatis jika terjadi exception saat eksekusi.
    /// Setiap retry akan dilakukan dengan delay bertahap sesuai pengaturan.
    /// </summary>
    /// <param name="notification">Notifikasi yang akan diproses.</param>
    /// <param name="handler">Delegate handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi async.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses pipeline notifikasi.</returns>
    public async Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                attempt => TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * attempt),
                (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(exception,
                        "Retry {Attempt} for notification {NotificationName} after {Delay}ms.",
                        attempt,
                        typeof(TNotification).Name,
                        delay.TotalMilliseconds);
                });

        await policy.ExecuteAsync(() => handler());
    }
}
