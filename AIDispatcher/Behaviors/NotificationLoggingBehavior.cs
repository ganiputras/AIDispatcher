using AIDispatcher.Notification;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIDispatcher.Behaviors;

/// <summary>
/// Behavior ini mencatat log sebelum dan sesudah handler notifikasi dijalankan.
/// Cocok untuk kebutuhan observasi/debugging alur notifikasi.
/// </summary>
public class LoggingNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : notnull
{
    private readonly ILogger<LoggingNotificationBehavior<TNotification>> _logger;

    public LoggingNotificationBehavior(ILogger<LoggingNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

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
/// Cocok untuk menghadapi error sementara seperti koneksi database atau API.
/// </summary>
public class RetryNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : notnull
{
    private readonly ILogger<RetryNotificationBehavior<TNotification>> _logger;
    private const int MaxRetries = 3;

    public RetryNotificationBehavior(ILogger<RetryNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

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

        await next(); // Final attempt
    }
}

/// <summary>
/// Behavior ini mencatat waktu eksekusi notifikasi (benchmark).
/// Cocok untuk memonitor performa sistem dan mendeteksi bottleneck handler lambat.
/// </summary>
public class MetricsNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : notnull
{
    private readonly ILogger<MetricsNotificationBehavior<TNotification>> _logger;

    public MetricsNotificationBehavior(ILogger<MetricsNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

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
