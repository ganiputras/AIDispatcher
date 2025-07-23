using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk menangani exception global pada permintaan (command/query) yang menghasilkan response.
/// Exception yang tidak tertangani akan dicatat ke log, lalu dilempar ulang sebagai <see cref="DispatcherException"/>.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="ExceptionBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat error saat eksekusi handler request.</param>
    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani pipeline request dengan mencatat dan melempar ulang exception dari handler request.
    /// Exception yang terjadi akan dicatat ke log, lalu dilempar ulang sebagai <see cref="DispatcherException"/>.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Response hasil pemrosesan handler.</returns>
    /// <exception cref="DispatcherException">Dilempar jika terjadi exception pada handler request.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing request {RequestType}.",
                typeof(TRequest).Name);
            throw new DispatcherException($"Failed to execute request {typeof(TRequest).Name}.", ex);
        }
    }
}

/// <summary>
/// Pipeline behavior untuk menangani exception global pada permintaan (command) tanpa hasil (void).
/// Exception yang tidak tertangani akan dicatat ke log, lalu dilempar ulang sebagai <see cref="DispatcherException"/>.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
public class ExceptionBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<ExceptionBehavior<TRequest>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="ExceptionBehavior{TRequest}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat error saat eksekusi handler request.</param>
    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani pipeline request dengan mencatat dan melempar ulang exception dari handler request.
    /// Exception yang terjadi akan dicatat ke log, lalu dilempar ulang sebagai <see cref="DispatcherException"/>.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler.</returns>
    /// <exception cref="DispatcherException">Dilempar jika terjadi exception pada handler request.</exception>
    public async Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing request {RequestType}.",
                typeof(TRequest).Name);
            throw new DispatcherException($"Failed to execute request {typeof(TRequest).Name}.", ex);
        }
    }
}
