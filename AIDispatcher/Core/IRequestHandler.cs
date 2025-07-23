namespace AIDispatcher.Core;

/// <summary>
///     Menangani permintaan dengan hasil.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang ditangani.</typeparam>
/// <typeparam name="TResponse">Tipe hasil yang dikembalikan.</typeparam>
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    ///     Menangani permintaan dan mengembalikan hasil.
    /// </summary>
    /// <param name="request">Permintaan yang diterima.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi.</param>
    /// <returns>Objek hasil sebagai respons.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
///     Menangani permintaan tanpa hasil.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang ditangani.</typeparam>
public interface IRequestHandler<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    ///     Menangani permintaan tanpa menghasilkan nilai balik.
    /// </summary>
    /// <param name="request">Permintaan yang diterima.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi.</param>
    /// <returns>Task yang merepresentasikan operasi asinkron.</returns>
    Task Handle(TRequest request, CancellationToken cancellationToken);
}