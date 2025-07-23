namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka penanda untuk request yang memiliki batas waktu eksekusi (timeout).
/// Jika diimplementasikan, request akan otomatis dibatalkan jika melebihi waktu maksimum yang ditentukan.
/// </summary>
public interface ITimeoutAware
{
    /// <summary>
    /// Waktu maksimal eksekusi request sebelum dianggap timeout dan dibatalkan.
    /// </summary>
    TimeSpan Timeout { get; }
}