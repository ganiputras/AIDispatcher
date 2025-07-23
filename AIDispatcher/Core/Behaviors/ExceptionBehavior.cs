using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk menangani exception global pada permintaan dengan hasil (command/query dengan response).
/// </summary>
public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> _logger;

    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing request {RequestType}.", typeof(TRequest).Name);
            throw new DispatcherException($"Failed to execute request {typeof(TRequest).Name}.", ex);
        }
    }
}

/// <summary>
///     Pipeline behavior untuk menangani exception global pada permintaan tanpa hasil (command void).
/// </summary>
public class ExceptionBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<ExceptionBehavior<TRequest>> _logger;

    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest>> logger)
        => _logger = logger;

    /// <inheritdoc />
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing request {RequestType}.", typeof(TRequest).Name);
            throw new DispatcherException($"Failed to execute request {typeof(TRequest).Name}.", ex);
        }
    }
}
