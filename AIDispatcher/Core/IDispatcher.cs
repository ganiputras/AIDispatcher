namespace AIDispatcher.Core;

/// <summary>
/// Kontrak utama untuk Dispatcher, mirip IMediator di MediatR.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Mengirim permintaan dengan hasil ke handler yang sesuai.
    /// </summary>
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;

    /// <summary>
    /// Mengirim permintaan tanpa hasil ke handler yang sesuai.
    /// </summary>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest;

    /// <summary>
    /// Mempublikasikan notifikasi ke semua handler terkait.
    /// </summary>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}