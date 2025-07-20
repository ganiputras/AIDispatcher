namespace AIDispatcher.Dispatcher;

/// <summary>
/// Antarmuka utama dispatcher.
/// Mirip dengan MediatR.IMediator, digunakan untuk mengirim permintaan ke handler yang terdaftar,
/// melalui pipeline behavior yang dikonfigurasi.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Mengirim permintaan (request) ke handler yang sesuai dan mengembalikan respons.
    /// Eksekusi akan melewati semua behavior yang telah dikonfigurasi (seperti validasi, logging, dll).
    /// </summary>
    /// <typeparam name="TRequest">Tipe permintaan.</typeparam>
    /// <typeparam name="TResponse">Tipe hasil dari permintaan.</typeparam>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token pembatalan async opsional.</param>
    /// <returns>Respons dari handler permintaan.</returns>
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);
}