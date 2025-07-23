using AIDispatcher.Core;
using AIDispatcher.Core.Interfaces;

/// <summary>
/// Behavior pipeline untuk mengeksekusi pre-processor sebelum handler dan post-processor setelah handler.
/// Pre-processor digunakan untuk menjalankan logika sebelum request diproses handler utama, sedangkan post-processor untuk logika setelah respons dihasilkan.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
public class PrePostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Daftar pre-processor yang akan dijalankan sebelum handler utama.
    /// </summary>
    private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;

    /// <summary>
    /// Daftar post-processor yang akan dijalankan setelah handler utama menghasilkan response.
    /// </summary>
    private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;

    /// <summary>
    /// Membuat instance baru dari <see cref="PrePostProcessorBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="preProcessors">Koleksi pre-processor untuk dijalankan sebelum handler.</param>
    /// <param name="postProcessors">Koleksi post-processor untuk dijalankan setelah handler.</param>
    public PrePostProcessorBehavior(
        IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
        IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
    {
        _preProcessors = preProcessors;
        _postProcessors = postProcessors;
    }

    /// <summary>
    /// Menangani eksekusi request dengan menjalankan pre-processor, handler utama, lalu post-processor secara berurutan.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler utama.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi async.</param>
    /// <returns>Objek response hasil pemrosesan.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var processor in _preProcessors)
            await processor.Process(request, cancellationToken);

        var response = await next();

        foreach (var processor in _postProcessors)
            await processor.Process(request, response, cancellationToken);

        return response;
    }
}
