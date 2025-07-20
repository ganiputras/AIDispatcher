using AIDispatcher.Notification;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Behavior ini mencatat log sebelum dan sesudah handler notifikasi dijalankan.
    /// Cocok untuk kebutuhan observasi dan debugging alur notifikasi.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
    public class LoggingNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
        where TNotification : notnull
    {
        private readonly ILogger<LoggingNotificationBehavior<TNotification>> _logger;

        /// <summary>
        /// Membuat instance LoggingNotificationBehavior dengan dependency logger.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat informasi dan error.</param>
        public LoggingNotificationBehavior(ILogger<LoggingNotificationBehavior<TNotification>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Menangani eksekusi notifikasi dengan mencatat log sebelum dan sesudah handler dijalankan.
        /// </summary>
        /// <param name="notification">Notifikasi yang sedang diproses.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <param name="next">Delegate untuk melanjutkan ke behavior atau handler berikutnya.</param>
        /// <returns>Task yang merepresentasikan eksekusi asynchronous.</returns>
        public async Task HandleAsync(
            TNotification notification,
            CancellationToken cancellationToken,
            Func<Task> next)
        {
            var name = typeof(TNotification).Name;
            _logger.LogInformation("Handling notification: {NotificationType}", name);

            try
            {
                await next();
                _logger.LogInformation("Handled notification: {NotificationType}", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling notification: {NotificationType}", name);
                throw;
            }
        }
    }

    /// <summary>
    /// Behavior ini secara otomatis mencoba ulang eksekusi handler notifikasi jika gagal (retry).
    /// Cocok untuk menghadapi error sementara seperti kegagalan koneksi database atau API.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
    public class RetryNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
        where TNotification : notnull
    {
        private readonly ILogger<RetryNotificationBehavior<TNotification>> _logger;
        private const int MaxRetries = 3;

        /// <summary>
        /// Membuat instance RetryNotificationBehavior dengan dependency logger.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat peringatan saat retry.</param>
        public RetryNotificationBehavior(ILogger<RetryNotificationBehavior<TNotification>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Menangani eksekusi notifikasi dengan mekanisme retry otomatis jika terjadi kegagalan.
        /// </summary>
        /// <param name="notification">Notifikasi yang sedang diproses.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <param name="next">Delegate untuk melanjutkan ke behavior atau handler berikutnya.</param>
        /// <returns>Task yang merepresentasikan eksekusi asynchronous.</returns>
        public async Task HandleAsync(
            TNotification notification,
            CancellationToken cancellationToken,
            Func<Task> next)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    await next();
                    return;
                }
                catch (Exception ex) when (attempt < MaxRetries)
                {
                    _logger.LogWarning(ex, "Retrying {NotificationType} attempt {Attempt}", typeof(TNotification).Name, attempt);
                    await Task.Delay(100 * attempt, cancellationToken);
                }
            }

            await next(); // Percobaan terakhir
        }
    }

    /// <summary>
    /// Behavior ini mencatat waktu eksekusi notifikasi (benchmark).
    /// Cocok untuk memonitor performa sistem dan mendeteksi bottleneck handler lambat.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
    public class MetricsNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
        where TNotification : notnull
    {
        private readonly ILogger<MetricsNotificationBehavior<TNotification>> _logger;

        /// <summary>
        /// Membuat instance MetricsNotificationBehavior dengan dependency logger.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat informasi durasi eksekusi.</param>
        public MetricsNotificationBehavior(ILogger<MetricsNotificationBehavior<TNotification>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Menangani eksekusi notifikasi dengan mengukur dan mencatat waktu eksekusi handler.
        /// </summary>
        /// <param name="notification">Notifikasi yang sedang diproses.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <param name="next">Delegate untuk melanjutkan ke behavior atau handler berikutnya.</param>
        /// <returns>Task yang merepresentasikan eksekusi asynchronous.</returns>
        public async Task HandleAsync(
            TNotification notification,
            CancellationToken cancellationToken,
            Func<Task> next)
        {
            var sw = Stopwatch.StartNew();
            await next();
            sw.Stop();

            _logger.LogInformation("Notification {Type} executed in {Elapsed} ms",
                typeof(TNotification).Name,
                sw.ElapsedMilliseconds);
        }
    }
}
