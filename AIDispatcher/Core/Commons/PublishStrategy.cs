namespace AIDispatcher.Core.Commons;

/// <summary>
/// Menentukan mode eksekusi handler notifikasi pada dispatcher.
/// Digunakan untuk mengatur apakah handler dieksekusi satu per satu atau secara paralel.
/// </summary>
public enum PublishStrategy
{
    /// <summary>
    /// Eksekusi handler notifikasi secara berurutan, satu per satu.
    /// Ini adalah mode default.
    /// </summary>
    Sequential,

    /// <summary>
    /// Eksekusi semua handler notifikasi secara paralel.
    /// Setiap handler akan dijalankan pada task terpisah secara bersamaan.
    /// </summary>
    Parallel
}