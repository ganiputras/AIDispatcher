namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Menandakan request mendukung percobaan ulang jika gagal.
/// </summary>
public interface IRetryAware
{
    /// <summary>Jumlah maksimal percobaan.</summary>
    int MaxRetryCount { get; }

    /// <summary>Jeda antar percobaan.</summary>
    TimeSpan Delay { get; }
}