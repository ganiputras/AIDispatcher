namespace AIDispatcher.Dispatcher;

/// <summary>
///     Opsi konfigurasi global untuk <c>Dispatcher</c>.
///     Digunakan untuk mengatur perilaku default seperti timeout, eksekusi paralel, dan prioritas handler notifikasi.
/// </summary>
public class DispatcherOptions
{
    /// <summary>
    ///     Timeout default untuk setiap permintaan.
    ///     Nilai default adalah 30 detik. Gunakan <c>Timeout.InfiniteTimeSpan</c> untuk menonaktifkan timeout.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Jika <c>true</c>, maka semua handler notifikasi akan dijalankan secara paralel (menggunakan <c>Task.WhenAll</c>).
    ///     Cocok untuk performa tinggi, namun pastikan handler tidak saling tergantung atau memiliki side effect.
    /// </summary>
    public bool ParallelNotificationHandlers { get; set; } = false;

    /// <summary>
    ///     Jika <c>true</c>, maka handler notifikasi akan dieksekusi berdasarkan urutan prioritas (dari interface <see cref="INotificationHandlerWithPriority" />).
    ///     Prioritas tertinggi akan dijalankan terlebih dahulu.
    ///     Fitur ini berguna jika urutan penanganan notifikasi penting.
    /// </summary>
    public bool NotificationHandlerPriorityEnabled { get; set; } = false;
}
