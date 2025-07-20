namespace AIDispatcher.Dispatcher;

/// <summary>
/// Kontrak untuk handler permintaan dengan respons.
/// Mirip dengan <c>MediatR.IRequestHandler</c>, tapi dirancang untuk digunakan dengan AIDispatcher.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang ditangani.</typeparam>
/// <typeparam name="TResponse">Tipe hasil/respons yang dikembalikan.</typeparam>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Menangani permintaan secara asinkron dan mengembalikan respons.
    /// </summary>
    /// <param name="request">Permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token pembatalan untuk menghentikan eksekusi.</param>
    /// <returns>Objek respons hasil dari pemrosesan permintaan.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}