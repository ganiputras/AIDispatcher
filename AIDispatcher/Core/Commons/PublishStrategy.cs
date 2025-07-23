namespace AIDispatcher.Core.Commons;

/// <summary>
///     Menentukan mode eksekusi handler notifikasi.
/// </summary>
public enum PublishStrategy
{
    /// <summary>
    ///     Eksekusi satu per satu (default).
    /// </summary>
    Sequential,

    /// <summary>
    ///     Eksekusi semua handler secara paralel.
    /// </summary>
    Parallel
}