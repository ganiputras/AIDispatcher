namespace AIDispatcher.Dispatcher;

/// <summary>
/// Antarmuka untuk pipeline behavior (middleware) dalam sistem dispatcher.
/// Behavior dapat digunakan untuk validasi, logging, retry, circuit breaker, dan lainnya.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan.</typeparam>
/// <typeparam name="TResponse">Tipe respons.</typeparam>
public interface IDispatcherBehavior<TRequest, TResponse>
{
    /// <summary>
    /// Menangani permintaan dan memanggil handler berikutnya di pipeline.
    /// </summary>
    /// <param name="request">Objek permintaan.</param>
    /// <param name="cancellationToken">Token pembatalan.</param>
    /// <param name="next">Delegasi ke handler selanjutnya dalam pipeline.</param>
    /// <returns>Respons dari permintaan.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken, DispatcherHandlerDelegate<TResponse> next);
}


