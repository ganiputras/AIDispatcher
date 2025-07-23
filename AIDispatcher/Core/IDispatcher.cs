namespace AIDispatcher.Core;

/// <summary>
/// Kontrak utama untuk Dispatcher, berfungsi sebagai mediator modern yang mengarahkan request dan notifikasi ke handler yang sesuai.
/// Konsepnya mirip <c>IMediator</c> di MediatR, namun lebih modular dan extensible.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Mengirim permintaan (request/command/query) dengan hasil ke handler yang sesuai,
    /// beserta seluruh pipeline behavior yang telah diregistrasi.
    /// </summary>
    /// <typeparam name="TRequest">Tipe permintaan yang akan diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe hasil yang diharapkan dari handler.</typeparam>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang menghasilkan response dari handler utama.</returns>
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;

    /// <summary>
    /// Mengirim permintaan tanpa hasil (void command) ke handler yang sesuai,
    /// beserta seluruh pipeline behavior yang telah diregistrasi.
    /// </summary>
    /// <typeparam name="TRequest">Tipe permintaan yang akan diproses.</typeparam>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses pengiriman permintaan.</returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest;

    /// <summary>
    /// Mempublikasikan notifikasi ke semua handler terkait,
    /// beserta seluruh notification pipeline behavior, dengan strategi eksekusi sesuai konfigurasi (sequential atau parallel).
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang akan diproses.</typeparam>
    /// <param name="notification">Objek notifikasi yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses publish notifikasi ke seluruh handler.</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
