namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Menyediakan mekanisme untuk memproses logika tambahan sebelum dan sesudah handler dijalankan.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan.</typeparam>
/// <typeparam name="TResponse">Tipe respons.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    ///     Menangani permintaan dengan logika tambahan di sekeliling handler utama.
    /// </summary>
    /// <param name="request">Permintaan yang diterima.</param>
    /// <param name="next">Delegasi ke langkah selanjutnya (handler berikut atau akhir pipeline).</param>
    /// <param name="cancellationToken">Token pembatalan.</param>
    /// <returns>Hasil dari eksekusi handler atau pipeline berikutnya.</returns>
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

/// <summary>
///     Menyediakan pipeline behavior untuk permintaan tanpa hasil (void/command).
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang ditangani.</typeparam>
public interface IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    ///     Menangani permintaan tanpa hasil dengan menyisipkan logika tambahan.
    /// </summary>
    /// <param name="request">Permintaan yang diterima.</param>
    /// <param name="next">Delegasi ke handler berikutnya.</param>
    /// <param name="cancellationToken">Token pembatalan.</param>
    /// <returns>Task dari operasi pipeline.</returns>
    Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken);
}