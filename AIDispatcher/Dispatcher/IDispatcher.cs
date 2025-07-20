/// <summary>
///     Antarmuka utama untuk dispatcher.
///     Mirip seperti <c>MediatR.IMediator</c>, digunakan untuk mengirim permintaan (request)
///     ke handler yang sesuai, dengan mendukung eksekusi melalui pipeline behavior (seperti validasi, logging, dll).
/// </summary>
public interface IDispatcher
{
    /// <summary>
    ///     Mengirim permintaan (request) ke handler yang sesuai dan mengembalikan respons.
    ///     Seluruh proses akan melewati urutan behavior (seperti validasi, logging, retry, circuit breaker, dll),
    ///     sebelum mencapai handler utama yang menangani permintaan.
    /// </summary>
    /// <typeparam name="TRequest">Tipe permintaan yang dikirim.</typeparam>
    /// <typeparam name="TResponse">Tipe respons yang dikembalikan dari handler.</typeparam>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token opsional untuk membatalkan operasi async.</param>
    /// <returns>Objek respons hasil dari pemrosesan handler.</returns>
    Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default);

}