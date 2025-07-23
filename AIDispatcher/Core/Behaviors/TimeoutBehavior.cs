using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk membatalkan eksekusi request tanpa hasil (void) jika waktu pemrosesan melebihi batas timeout yang ditentukan oleh <see cref="ITimeoutAware"/>.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
public class TimeoutBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<TimeoutBehavior<TRequest>> _logger;

    /// <summary>
    /// Membuat instance baru <see cref="TimeoutBehavior{TRequest}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat peristiwa timeout.</param>
    public TimeoutBehavior(ILogger<TimeoutBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani request dengan menambahkan mekanisme pembatalan otomatis jika waktu eksekusi melebihi nilai <see cref="ITimeoutAware.Timeout"/>.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi.</param>
    /// <returns>Task yang merepresentasikan operasi asynchronous.</returns>
    /// <exception cref="TimeoutException">Dilempar jika eksekusi melebihi waktu yang diizinkan.</exception>
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        if (request is not ITimeoutAware timeoutAware)
        {
            await next();
            return;
        }

        using var timeoutCts = new CancellationTokenSource(timeoutAware.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await next().WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Request {RequestName} timed out after {TimeoutSeconds} seconds.",
                typeof(TRequest).Name,
                timeoutAware.Timeout.TotalSeconds
            );
            throw new TimeoutException(
                $"Request {typeof(TRequest).Name} exceeded the timeout of {timeoutAware.Timeout.TotalSeconds} seconds.");
        }
    }
}

/// <summary>
/// Pipeline behavior untuk membatalkan eksekusi request dengan hasil (response) jika waktu pemrosesan melebihi batas timeout yang ditentukan oleh <see cref="ITimeoutAware"/>.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
public class TimeoutBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TimeoutBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Membuat instance baru <see cref="TimeoutBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat peristiwa timeout.</param>
    public TimeoutBehavior(ILogger<TimeoutBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani request dengan menambahkan mekanisme pembatalan otomatis jika waktu eksekusi melebihi nilai <see cref="ITimeoutAware.Timeout"/>.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi.</param>
    /// <returns>Objek response hasil pemrosesan.</returns>
    /// <exception cref="TimeoutException">Dilempar jika eksekusi melebihi waktu yang diizinkan.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITimeoutAware timeoutAware)
            return await next();

        using var timeoutCts = new CancellationTokenSource(timeoutAware.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            return await next().WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Request {RequestName} timed out after {TimeoutSeconds} seconds.",
                typeof(TRequest).Name,
                timeoutAware.Timeout.TotalSeconds
            );
            throw new TimeoutException(
                $"Request {typeof(TRequest).Name} exceeded the timeout of {timeoutAware.Timeout.TotalSeconds} seconds.");
        }
    }
}
