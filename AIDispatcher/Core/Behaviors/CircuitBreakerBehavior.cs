using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk menerapkan pola circuit breaker pada eksekusi handler request (command/query) dengan response.
/// Jika terjadi error secara berturut-turut, eksekusi handler akan diputus (break) sementara waktu sesuai pengaturan.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan oleh handler.</typeparam>
public class CircuitBreakerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly ILogger<CircuitBreakerBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="CircuitBreakerBehavior{TRequest, TResponse}"/> dengan konfigurasi circuit breaker.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat status dan error circuit breaker.</param>
    /// <param name="exceptionsAllowedBeforeBreaking">Jumlah maksimum exception yang diperbolehkan sebelum circuit breaker aktif (default: 2).</param>
    /// <param name="durationOfBreakMs">Durasi waktu (dalam ms) circuit breaker akan aktif sebelum reset (default: 2000 ms).</param>
    public CircuitBreakerBehavior(
        ILogger<CircuitBreakerBehavior<TRequest, TResponse>> logger,
        int exceptionsAllowedBeforeBreaking = 2,
        int durationOfBreakMs = 2000)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromMilliseconds(durationOfBreakMs),
                (ex, breakDelay) =>
                {
                    _logger.LogWarning(ex, "Circuit broken for {BreakDelay}ms after exception in {RequestName}.",
                        breakDelay.TotalMilliseconds, typeof(TRequest).Name);
                },
                () => { _logger.LogInformation("Circuit reset for {RequestName}.", typeof(TRequest).Name); });
    }

    /// <summary>
    /// Menangani pipeline dengan menerapkan mekanisme circuit breaker pada handler request.
    /// Jika terjadi exception secara berturut-turut melebihi batas yang ditentukan, eksekusi handler akan diputus sementara.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Response hasil pemrosesan handler.</returns>
    /// <exception cref="BrokenCircuitException">Dilempar jika circuit breaker sedang aktif (open state).</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await _circuitBreakerPolicy.ExecuteAsync(() => next());
    }
}

/// <summary>
/// Pipeline behavior untuk menerapkan pola circuit breaker pada eksekusi handler request tanpa hasil (void command).
/// Jika terjadi error secara berturut-turut, eksekusi handler akan diputus (break) sementara waktu sesuai pengaturan.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
public class CircuitBreakerBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly ILogger<CircuitBreakerBehavior<TRequest>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="CircuitBreakerBehavior{TRequest}"/> dengan konfigurasi circuit breaker.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat status dan error circuit breaker.</param>
    /// <param name="exceptionsAllowedBeforeBreaking">Jumlah maksimum exception yang diperbolehkan sebelum circuit breaker aktif (default: 2).</param>
    /// <param name="durationOfBreakMs">Durasi waktu (dalam ms) circuit breaker akan aktif sebelum reset (default: 2000 ms).</param>
    public CircuitBreakerBehavior(
        ILogger<CircuitBreakerBehavior<TRequest>> logger,
        int exceptionsAllowedBeforeBreaking = 2,
        int durationOfBreakMs = 2000)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromMilliseconds(durationOfBreakMs),
                (ex, breakDelay) =>
                {
                    _logger.LogWarning(ex, "Circuit broken for {BreakDelay}ms after exception in {RequestName}.",
                        breakDelay.TotalMilliseconds, typeof(TRequest).Name);
                },
                () => { _logger.LogInformation("Circuit reset for {RequestName}.", typeof(TRequest).Name); });
    }

    /// <summary>
    /// Menangani pipeline dengan menerapkan mekanisme circuit breaker pada handler request.
    /// Jika terjadi exception secara berturut-turut melebihi batas yang ditentukan, eksekusi handler akan diputus sementara.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler.</returns>
    /// <exception cref="BrokenCircuitException">Dilempar jika circuit breaker sedang aktif (open state).</exception>
    public async Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        await _circuitBreakerPolicy.ExecuteAsync(() => next());
    }
}
