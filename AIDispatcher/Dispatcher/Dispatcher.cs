using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace AIDispatcher.Dispatcher;

/// <summary>
///     Implementasi default dari <see cref="IDispatcher" />.
///     Menyusun dan mengeksekusi pipeline behavior sebelum memanggil handler yang sesuai.
/// </summary>
/// <remarks>
///     Kelas ini mengambil handler dan behavior dari dependency injection (DI), lalu menyusunnya
///     menjadi rantai eksekusi yang berurutan. Setiap behavior membungkus eksekusi berikutnya
///     untuk memungkinkan penerapan concern lintas (cross-cutting) seperti logging, validasi, retry, dll.
/// </remarks>
public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    // Cache delegate pipeline per pasangan request-response agar tidak perlu menyusun ulang pipeline setiap waktu
    private static readonly ConcurrentDictionary<(Type, Type), object> _delegateCache = new();

    /// <summary>
    ///     Inisialisasi instance baru dari <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" /> yang digunakan untuk resolve handler dan behavior dari DI container.
    /// </param>
    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = (typeof(TRequest), typeof(TResponse));

        if (!_delegateCache.TryGetValue(key, out var cachedDelegate))
        {
            // Ambil handler dan semua behavior yang relevan dari DI container
            var handler = _serviceProvider.GetRequiredService<IDispatcherHandler<TRequest, TResponse>>();
            var behaviors = _serviceProvider
                .GetServices<IDispatcherBehavior<TRequest, TResponse>>()
                .ToList();

            // Delegate akhir yang langsung memanggil handler
            DispatcherHandlerDelegate<TResponse> handlerDelegate = ct => handler.HandleAsync(request, ct);

            // Bungkus dengan behavior secara berurutan (dibalik untuk urutan eksekusi yang benar)
            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = handlerDelegate;
                handlerDelegate = ct => behavior.HandleAsync(request, next, ct);
            }

            // Simpan ke cache agar tidak perlu dirakit ulang
            _delegateCache[key] = handlerDelegate;
            cachedDelegate = handlerDelegate;
        }

        var finalDelegate = (DispatcherHandlerDelegate<TResponse>)cachedDelegate;
        return await finalDelegate(cancellationToken);
    }

}
