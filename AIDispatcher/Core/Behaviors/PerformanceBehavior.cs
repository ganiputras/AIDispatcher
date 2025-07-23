using System.Diagnostics;
using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk mencatat durasi eksekusi handler request (performance monitoring).
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly int _thresholdMs;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IOptions<DispatcherOptions> options)
    {
        _logger = logger;
        _thresholdMs = options.Value.PerformanceThresholdMs;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
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
///     Pipeline behavior untuk memonitor dan mencatat durasi eksekusi handler permintaan tanpa hasil (command void).
///     Memberikan warning di log jika eksekusi melebihi ambang batas yang ditentukan.
/// </summary>
/// <typeparam name="TRequest">Tipe permintaan (tanpa response).</typeparam>
public class PerformanceBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<PerformanceBehavior<TRequest>> _logger;
    private readonly int _thresholdMs;

    /// <summary>
    ///     Inisialisasi <see cref="PerformanceBehavior{TRequest}" />.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat informasi dan warning.</param>
    /// <param name="options">Opsi dispatcher, ambil nilai threshold.</param>
    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest>> logger,
        IOptions<DispatcherOptions> options)
    {
        _logger = logger;
        _thresholdMs = options.Value.PerformanceThresholdMs;
    }

    /// <inheritdoc />
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
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