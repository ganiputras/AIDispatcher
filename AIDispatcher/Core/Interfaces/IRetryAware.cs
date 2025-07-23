namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka penanda untuk request yang mendukung percobaan ulang (retry) secara otomatis jika eksekusi gagal.
/// Jika diimplementasikan, pipeline behavior dapat membaca konfigurasi retry ini dan mengatur jumlah percobaan serta jeda antar percobaan.
/// </summary>
public interface IRetryAware
{
    /// <summary>
    /// Jumlah maksimal percobaan ulang (retry) yang diizinkan jika request gagal.
    /// </summary>
    int MaxRetryCount { get; }

    /// <summary>
    /// Jeda waktu antara setiap percobaan ulang (retry).
    /// Biasanya digunakan untuk memberikan waktu jeda sebelum mencoba kembali eksekusi request.
    /// </summary>
    TimeSpan Delay { get; }
}