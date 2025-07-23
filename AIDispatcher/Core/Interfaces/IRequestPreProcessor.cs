namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka untuk pre-processor pada pipeline dispatcher.
/// Digunakan untuk menjalankan logika tambahan sebelum handler utama dijalankan (pre-processing).
/// Cocok untuk kebutuhan seperti validasi, logging awal, inisialisasi data, atau autentikasi.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan (request) yang diproses.</typeparam>
public interface IRequestPreProcessor<in TRequest>
{
    /// <summary>
    /// Menangani logika tambahan sebelum handler dijalankan.
    /// </summary>
    /// <param name="request">Objek permintaan (request) yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses pre-processing.</returns>
    Task Process(
        TRequest request,
        CancellationToken cancellationToken);
}