using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk melakukan retry otomatis jika terjadi exception pada eksekusi handler request dengan response.
/// Retry dilakukan menggunakan policy dengan jumlah percobaan dan jeda antar retry yang dapat diatur.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan oleh handler.</typeparam>
public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    /// <summary>
    /// Membuat instance baru dari <see cref="RetryBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat proses retry.</param>
    /// <param name="retryCount">Jumlah maksimum percobaan retry saat terjadi exception (default 3 kali).</param>
    /// <param name="delayMs">Delay (dalam milidetik) antar percobaan retry (default 200 ms).</param>
    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger, int retryCount = 3, int delayMs = 200)
    {
        _logger = logger;
        _retryCount = retryCount;
        _retryDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    /// <summary>
    /// Menangani pipeline handler dengan mekanisme retry otomatis jika terjadi exception.
    /// Setiap retry akan dilakukan dengan delay bertahap sesuai pengaturan.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Response hasil pemrosesan handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                attempt => TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * attempt),
                (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {Attempt} for {RequestName} after {Delay}ms.",
                        attempt,
                        typeof(TRequest).Name,
                        delay.TotalMilliseconds);
                });

        return await policy.ExecuteAsync(() => next());
    }
}

/// <summary>
/// Pipeline behavior untuk melakukan retry otomatis jika terjadi exception pada eksekusi handler request tanpa hasil (void).
/// Retry dilakukan menggunakan policy dengan jumlah percobaan dan jeda antar retry yang dapat diatur.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
public class RetryBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<RetryBehavior<TRequest>> _logger;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    /// <summary>
    /// Membuat instance baru dari <see cref="RetryBehavior{TRequest}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat proses retry.</param>
    /// <param name="retryCount">Jumlah maksimum percobaan retry saat terjadi exception (default 3 kali).</param>
    /// <param name="delayMs">Delay (dalam milidetik) antar percobaan retry (default 200 ms).</param>
    public RetryBehavior(ILogger<RetryBehavior<TRequest>> logger, int retryCount = 3, int delayMs = 200)
    {
        _logger = logger;
        _retryCount = retryCount;
        _retryDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    /// <summary>
    /// Menangani pipeline handler dengan mekanisme retry otomatis jika terjadi exception.
    /// Setiap retry akan dilakukan dengan delay bertahap sesuai pengaturan.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler.</returns>
    public async Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                attempt => TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * attempt),
                (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {Attempt} for {RequestName} after {Delay}ms.",
                        attempt,
                        typeof(TRequest).Name,
                        delay.TotalMilliseconds);
                });

        await policy.ExecuteAsync(() => next());
    }
}
