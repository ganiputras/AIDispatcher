namespace AIDispatcher.Dispatcher;

/// <summary>
///     Antarmuka untuk handler utama sebuah permintaan (request).
///     Setiap handler bertugas menangani satu jenis permintaan dan mengembalikan hasilnya.
///     Mirip seperti command handler atau query handler dalam pola CQRS.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang akan diproses.</typeparam>
/// <typeparam name="TResponse">Tipe hasil (respons) yang akan dikembalikan.</typeparam>
public interface IDispatcherHandler<in TRequest, TResponse>
{
    /// <summary>
    ///     Menangani permintaan utama dan mengembalikan hasilnya.
    ///     Handler ini akan dipanggil setelah semua pipeline behavior selesai dieksekusi.
    /// </summary>
    /// <param name="request">Objek permintaan yang sedang diproses.</param>
    /// <param name="cancellationToken">Token untuk pembatalan proses asynchronous.</param>
    /// <returns>Objek hasil dari permintaan.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}