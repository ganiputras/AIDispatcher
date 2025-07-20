using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Dispatcher;

/// <summary>
///     Implementasi default dari <see cref="IDispatcher" />.
///     Menyusun dan menjalankan pipeline behaviors sebelum mengeksekusi handler permintaan.
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="serviceProvider">Penyedia layanan DI untuk resolving handler dan behavior.</param>
    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetRequiredService<IDispatcherHandler<TRequest, TResponse>>();
        var behaviors = _serviceProvider.GetServices<IDispatcherBehavior<TRequest, TResponse>>().ToList();

        Func<CancellationToken, Task<TResponse>> handlerDelegate = ct => handler.HandleAsync(request, ct);

        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            handlerDelegate = ct => behavior.HandleAsync(request, ct, next);
        }

        // Jalankan pipeline
        return await handlerDelegate(cancellationToken);
    }
}