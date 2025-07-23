namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka untuk pipeline behavior yang memungkinkan penyisipan logika tambahan sebelum dan sesudah handler utama dijalankan.
/// Cocok untuk kebutuhan cross-cutting seperti logging, validasi, retry, performance monitoring, dsb.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang akan diproses.</typeparam>
/// <typeparam name="TResponse">Tipe respons yang dihasilkan oleh handler utama.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Menangani permintaan dengan menyisipkan logika tambahan di sekeliling handler utama,
    /// seperti logging, exception handling, validasi, atau lainnya.
    /// </summary>
    /// <param name="request">Permintaan yang diterima.</param>
    /// <param name="next">Delegasi ke langkah berikutnya dalam pipeline (handler berikut atau akhir pipeline).</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Response hasil eksekusi handler atau pipeline berikutnya.</returns>
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Antarmuka untuk pipeline behavior pada permintaan tanpa hasil (void command),
/// memungkinkan penyisipan logika tambahan di sekitar handler utama.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan yang ditangani.</typeparam>
public interface IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Menangani permintaan tanpa hasil dengan menyisipkan logika tambahan sebelum dan/atau sesudah handler dijalankan,
    /// seperti logging, exception handling, validasi, atau lainnya.
    /// </summary>
    /// <param name="request">Permintaan yang diterima.</param>
    /// <param name="next">Delegasi ke handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan eksekusi pipeline.</returns>
    Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken);
}
