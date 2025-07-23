namespace AIDispatcher.Core.Commons;

/// <summary>
/// Opsi konfigurasi utama untuk Dispatcher.
/// Digunakan untuk mengatur perilaku eksekusi notifikasi dan ambang batas performa pipeline.
/// </summary>
public class DispatcherOptions
{
    /// <summary>
    /// Menentukan mode eksekusi notifikasi yang akan digunakan oleh dispatcher.
    /// Nilai default adalah <see cref="PublishStrategy.Sequential"/>.
    /// </summary>
    public PublishStrategy PublishStrategy { get; set; } = PublishStrategy.Sequential;

    /// <summary>
    /// Ambang batas waktu eksekusi (dalam milidetik) sebelum pipeline performance mencatat warning ke log.
    /// Nilai default adalah 500 ms.
    /// </summary>
    public int PerformanceThresholdMs { get; set; } = 500;
}