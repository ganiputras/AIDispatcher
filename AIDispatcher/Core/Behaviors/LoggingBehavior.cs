using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
/// Pipeline behavior untuk mencatat waktu mulai, durasi, dan waktu selesai eksekusi permintaan (request) yang menghasilkan response.
/// Informasi dicatat ke log dengan format profesional.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
/// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="LoggingBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat aktivitas pipeline.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani eksekusi pipeline untuk request dengan response, mencatat waktu mulai, waktu selesai, dan durasi eksekusi ke log.
    /// </summary>
    /// <param name="request">Request yang akan diproses.</param>
    /// <param name="next">Delegate handler berikutnya dalam pipeline.</param>
    /// <param name="ct">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Response hasil pemrosesan handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} started.", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation("Handling {RequestName} completed in {ElapsedMilliseconds} ms.",
            requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}

/// <summary>
/// Pipeline behavior untuk mencatat waktu mulai, durasi, dan waktu selesai eksekusi permintaan (request) tanpa response (command void).
/// Informasi dicatat ke log dengan format profesional.
/// </summary>
/// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<LoggingBehavior<TRequest>> _logger;

    /// <summary>
    /// Membuat instance baru dari <see cref="LoggingBehavior{TRequest}"/>.
    /// </summary>
    /// <param name="logger">Logger untuk mencatat aktivitas pipeline.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Menangani eksekusi pipeline untuk request tanpa response, mencatat waktu mulai, waktu selesai, dan durasi eksekusi ke log.
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
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} started.", requestName);

        var stopwatch = Stopwatch.StartNew();
        await next();
        stopwatch.Stop();

        _logger.LogInformation("Handling {RequestName} completed in {ElapsedMilliseconds} ms.",
            requestName,
            stopwatch.ElapsedMilliseconds);
    }
}
