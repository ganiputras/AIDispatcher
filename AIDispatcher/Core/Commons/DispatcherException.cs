namespace AIDispatcher.Core.Commons;

/// <summary>
/// Exception khusus yang digunakan untuk menangani kesalahan internal dalam proses Dispatcher.
/// Exception ini umumnya dilempar ketika terjadi error yang tidak terduga pada pipeline atau handler dispatcher.
/// </summary>
public class DispatcherException : Exception
{
    /// <summary>
    /// Membuat instance baru dari <see cref="DispatcherException"/> dengan pesan error dan inner exception (jika ada).
    /// </summary>
    /// <param name="message">Pesan error yang menjelaskan alasan exception.</param>
    /// <param name="innerException">Exception asli yang menyebabkan error (jika ada), boleh null.</param>
    public DispatcherException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}