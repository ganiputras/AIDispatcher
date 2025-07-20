namespace AIDispatcher.Dispatcher;

/// <summary>
///     Exception khusus yang digunakan untuk menangani kesalahan tak terduga 
///     selama eksekusi pipeline dispatcher.
///     Exception ini memfasilitasi pencatatan log yang konsisten, pelacakan telemetri,
///     serta diagnosis terpusat terhadap error pada proses <c>SendAsync</c>.
/// </summary>
public class DispatcherUnhandledException : Exception
{
    /// <summary>
    ///     Membuat instance baru dari <see cref="DispatcherUnhandledException"/>.
    /// </summary>
    /// <param name="requestType">
    ///     Tipe request yang menyebabkan exception ini terjadi.
    ///     Informasi ini membantu dalam pencatatan error dan pelacakan sumber masalah.
    /// </param>
    /// <param name="innerException">
    ///     Exception asli yang dilemparkan oleh handler atau behavior dalam pipeline dispatcher.
    /// </param>
    public DispatcherUnhandledException(Type requestType, Exception innerException)
        : base($"Unhandled exception during processing request of type '{requestType.Name}'", innerException)
    {
        RequestType = requestType;
    }

    /// <summary>
    ///     Tipe request yang menyebabkan exception ini muncul.
    ///     Properti ini bisa digunakan untuk logging atau analisis error.
    /// </summary>
    public Type RequestType { get; }
}