namespace AIDispatcher.Dispatcher;

/// <summary>
/// Adapter internal yang memungkinkan handler berbasis <see cref="IRequestHandler{TRequest,TResponse}"/>
/// digunakan dalam pipeline <see cref="IDispatcherHandler{TRequest,TResponse}"/> milik AIDispatcher.
/// </summary>
/// <remarks>
/// Dengan adanya adapter ini, developer dapat menggunakan gaya penulisan handler ala MediatR
/// tanpa perlu menulis ulang ke dalam bentuk <c>IDispatcherHandler</c>.
/// Adapter ini dijalankan secara otomatis selama registrasi dan tidak perlu digunakan secara manual.
///
/// Keuntungan:
/// - 🧩 Kompatibel dengan handler gaya MediatR
/// - 🔄 Mempermudah migrasi dari MediatR ke AIDispatcher
/// - ✅ Tetap menjaga desain internal AIDispatcher yang berbasis modular pipeline
/// </remarks>
/// <typeparam name="TRequest">Tipe request yang dikirimkan.</typeparam>
/// <typeparam name="TResponse">Tipe response yang diharapkan.</typeparam>
public class RequestHandlerAdapter<TRequest, TResponse>
    : IDispatcherHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _inner;

    /// <summary>
    /// Membuat instance adapter yang membungkus <see cref="IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="inner">
    /// Instance handler gaya MediatR yang akan dibungkus dan dijalankan oleh pipeline AIDispatcher.
    /// </param>
    public RequestHandlerAdapter(IRequestHandler<TRequest, TResponse> inner)
    {
        _inner = inner;
    }

    /// <summary>
    /// Menangani permintaan dengan meneruskan eksekusi ke handler <see cref="IRequestHandler{TRequest,TResponse}"/> yang dibungkus.
    /// </summary>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk menghentikan eksekusi secara kooperatif.</param>
    /// <returns>Task yang merepresentasikan hasil pemrosesan permintaan.</returns>
    public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        return _inner.HandleAsync(request, cancellationToken);
    }


}
