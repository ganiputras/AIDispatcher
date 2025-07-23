namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka untuk post-processor pada pipeline dispatcher.
/// Digunakan untuk menjalankan logika tambahan setelah handler utama selesai dijalankan (post-processing).
/// Cocok untuk kebutuhan seperti logging, audit, cache update, atau pengolahan hasil response.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan (request) yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe hasil response dari handler utama.</typeparam>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    /// <summary>
    /// Menangani logika tambahan setelah handler selesai dijalankan.
    /// </summary>
    /// <param name="request">Objek permintaan (request) yang diproses.</param>
    /// <param name="response">Response yang dihasilkan oleh handler utama.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses post-processing.</returns>
    Task Process(
        TRequest request,
        TResponse response,
        CancellationToken cancellationToken);
}