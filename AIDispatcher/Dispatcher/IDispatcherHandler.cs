namespace AIDispatcher.Dispatcher;

/// <summary>
///     Antarmuka untuk handler utama sebuah permintaan.
/// </summary>
/// <typeparam name="TRequest">Tipe request.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dikembalikan.</typeparam>
public interface IDispatcherHandler<in TRequest, TResponse>
{
    /// <summary>
    ///     Menangani request utama dan mengembalikan response.
    /// </summary>
    /// <param name="request">Objek request.</param>
    /// <param name="cancellationToken">Token pembatalan.</param>
    /// <returns>Objek response.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}