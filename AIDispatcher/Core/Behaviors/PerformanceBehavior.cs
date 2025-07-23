using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk memonitor dan mencatat durasi eksekusi handler request.
/// Jika waktu eksekusi melebihi ambang batas yang ditentukan, akan mencatat warning ke log.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly int _thresholdMs;

    /// <summary>
    /// Membuat instance baru dari <see cref="PerformanceBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat informasi dan warning performa.</param>
    /// <param name="options">Opsi konfigurasi dispatcher untuk ambang batas performa (threshold).</param>
    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IOptions<DispatcherOptions> options)
    {
        _logger = logger;
        _thresholdMs = options.Value.PerformanceThresholdMs;
    }

    /// <summary>
    /// Menangani pipeline dengan memonitor durasi eksekusi handler dan mencatat log informasi serta warning jika melebihi threshold.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi async.</param>
    /// <returns>Response hasil pemrosesan handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName} started.", requestName);

        var response = await next();

        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation("Handling {RequestName} completed in {ElapsedMilliseconds} ms.", requestName, elapsedMs);

        if (elapsedMs > _thresholdMs)
            _logger.LogWarning(
                "Performance warning: {RequestName} took {ElapsedMilliseconds} ms (threshold: {Threshold} ms).",
                requestName, elapsedMs, _thresholdMs);

        return response;
    }
}

/// <summary>
/// Pipeline behavior untuk memonitor dan mencatat durasi eksekusi handler permintaan tanpa hasil (void/command).
/// Mencatat warning di log jika durasi eksekusi melebihi ambang batas yang ditentukan.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan (tanpa response).</typeparam>
public class PerformanceBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<PerformanceBehavior<TRequest>> _logger;
    private readonly int _thresholdMs;

    /// <summary>
    /// Membuat instance baru dari <see cref="PerformanceBehavior{TRequest}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat informasi dan warning performa.</param>
    /// <param name="options">Opsi konfigurasi dispatcher untuk ambang batas performa (threshold).</param>
    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest>> logger,
        IOptions<DispatcherOptions> options)
    {
        _logger = logger;
        _thresholdMs = options.Value.PerformanceThresholdMs;
    }

    /// <summary>
    /// Menangani pipeline dengan memonitor durasi eksekusi handler dan mencatat log informasi serta warning jika melebihi threshold.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi async.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses handler.</returns>
    public async Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName} started.", requestName);

        await next();

        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation("Handling {RequestName} completed in {ElapsedMilliseconds} ms.", requestName, elapsedMs);

        if (elapsedMs > _thresholdMs)
            _logger.LogWarning(
                "Performance warning: {RequestName} took {ElapsedMilliseconds} ms (threshold: {Threshold} ms).",
                requestName, elapsedMs, _thresholdMs);
    }
}
