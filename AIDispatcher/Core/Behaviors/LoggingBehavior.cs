using System.Diagnostics;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk mencatat durasi eksekusi permintaan dengan response.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Menangani eksekusi pipeline untuk request dengan response, mencatat waktu mulai dan selesai dengan informasi
    ///     profesional.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} started.", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation("Handling {RequestName} completed in {ElapsedMilliseconds} ms.", requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}

/// <summary>
///     Pipeline behavior untuk mencatat durasi eksekusi permintaan tanpa hasil (command void).
/// </summary>
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<LoggingBehavior<TRequest>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Menangani eksekusi pipeline untuk request tanpa response, mencatat waktu mulai dan selesai dengan informasi
    ///     profesional.
    /// </summary>
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} started.", requestName);

        var stopwatch = Stopwatch.StartNew();
        await next();
        stopwatch.Stop();

        _logger.LogInformation("Handling {RequestName} completed in {ElapsedMilliseconds} ms.", requestName,
            stopwatch.ElapsedMilliseconds);
    }
}