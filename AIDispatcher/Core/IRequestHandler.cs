namespace AIDispatcher.Core;

/// <summary>
/// Antarmuka handler untuk menangani permintaan (request/command/query) yang menghasilkan respons.
/// Digunakan pada pola request/response agar dispatcher dapat mengarahkan permintaan ke handler dan mengembalikan hasil.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang ditangani.</typeparam>
/// <typeparam name="TResponse">Tipe hasil (response) yang dikembalikan oleh handler.</typeparam>
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Menangani permintaan dan mengembalikan hasil respons.
    /// </summary>
    /// <param name="request">Objek permintaan yang diterima handler.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Objek hasil (response) dari proses permintaan.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Antarmuka handler untuk menangani permintaan (command) tanpa hasil (void).
/// Digunakan untuk skenario perintah (command) yang hanya memicu aksi tanpa mengembalikan nilai balik.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan (command) yang ditangani.</typeparam>
public interface IRequestHandler<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Menangani permintaan tanpa menghasilkan nilai balik.
    /// </summary>
    /// <param name="request">Objek permintaan yang diterima handler.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses eksekusi handler.</returns>
    Task Handle(TRequest request, CancellationToken cancellationToken);
}