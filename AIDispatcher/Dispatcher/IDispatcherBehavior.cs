namespace AIDispatcher.Dispatcher;

/// <summary>
///     Antarmuka untuk pipeline behavior (middleware) dalam sistem dispatcher.
///     Setiap behavior dapat digunakan untuk menambahkan logika tambahan sebelum atau sesudah handler utama dieksekusi,
///     seperti validasi, logging, retry, circuit breaker, caching, dan lain-lain.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan (request) yang sedang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe hasil (response) yang dikembalikan oleh handler.</typeparam>
public interface IDispatcherBehavior<TRequest, TResponse>
{
    /// <summary>
    ///     Menangani permintaan dan memanggil handler berikutnya dalam urutan pipeline.
    ///     Metode ini memungkinkan behavior untuk menyisipkan logika tambahan sebelum atau sesudah <paramref name="next"/> dipanggil.
    /// </summary>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="next">
    ///     Delegasi ke handler selanjutnya dalam pipeline. Biasanya akan memanggil behavior berikutnya,
    ///     atau langsung ke handler utama jika tidak ada behavior lain.
    /// </param>
    /// <param name="cancellationToken">Token untuk pembatalan eksekusi secara asynchronous.</param>
    /// <returns>Objek respons hasil dari eksekusi pipeline.</returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        DispatcherHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}