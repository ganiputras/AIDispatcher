namespace AIDispatcher.Core.Commons;

/// <summary>
///     Opsi konfigurasi utama untuk Dispatcher.
/// </summary>
public class DispatcherOptions
{
    /// <summary>
    ///     Menentukan mode eksekusi notifikasi (default: Sequential).
    /// </summary>
    public PublishStrategy PublishStrategy { get; set; } = PublishStrategy.Sequential;

    /// <summary>
    ///     Ambang batas waktu eksekusi (dalam milidetik) sebelum log warning pada pipeline performance.
    ///     Default: 500 ms.
    /// </summary>
    public int PerformanceThresholdMs { get; set; } = 500;
}