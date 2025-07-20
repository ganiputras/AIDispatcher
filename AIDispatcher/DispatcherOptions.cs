namespace AIDispatcher;

/// <summary>
///     Options for configuring the dispatcher globally.
/// </summary>
public class DispatcherOptions
{
    /// <summary>
    ///     The default timeout for each request (defaults to 30 seconds).
    ///     Use Timeout.InfiniteTimeSpan to disable.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Jika true, maka notifikasi akan dieksekusi secara paralel (parallel Task).
    /// </summary>
    public bool ParallelNotificationHandlers { get; set; } = false;

    /// <summary>
    /// Jika true, maka notifikasi akan dijalankan berdasarkan prioritas (dari interface INotificationHandlerWithPriority).
    /// </summary>
    public bool NotificationHandlerPriorityEnabled { get; set; } = false;
}