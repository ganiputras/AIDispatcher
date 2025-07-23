using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AIDispatcher.Core.Behaviors;

/// <summary>
///     Pipeline behavior untuk circuit breaker pada eksekusi handler request.
/// </summary>
public class CircuitBreakerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<CircuitBreakerBehavior<TRequest, TResponse>> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public CircuitBreakerBehavior(ILogger<CircuitBreakerBehavior<TRequest, TResponse>> logger, int exceptionsAllowedBeforeBreaking = 2, int durationOfBreakMs = 2000)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromMilliseconds(durationOfBreakMs),
                (ex, breakDelay) =>
                {
                    _logger.LogWarning(ex, "Circuit broken for {BreakDelay}ms after exception in {RequestName}.", breakDelay.TotalMilliseconds, typeof(TRequest).Name);
                },
                () =>
                {
                    _logger.LogInformation("Circuit reset for {RequestName}.", typeof(TRequest).Name);
                });
    }

    /// <summary>
    ///     Menangani pipeline dengan circuit breaker, memutus eksekusi jika error terus-menerus.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return await _circuitBreakerPolicy.ExecuteAsync(() => next());
    }
}


/// <summary>
///     Pipeline behavior untuk circuit breaker pada handler request tanpa hasil (void command).
/// </summary>
public class CircuitBreakerBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    private readonly ILogger<CircuitBreakerBehavior<TRequest>> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public CircuitBreakerBehavior(ILogger<CircuitBreakerBehavior<TRequest>> logger, int exceptionsAllowedBeforeBreaking = 2, int durationOfBreakMs = 2000)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                TimeSpan.FromMilliseconds(durationOfBreakMs),
                (ex, breakDelay) =>
                {
                    _logger.LogWarning(ex, "Circuit broken for {BreakDelay}ms after exception in {RequestName}.", breakDelay.TotalMilliseconds, typeof(TRequest).Name);
                },
                () =>
                {
                    _logger.LogInformation("Circuit reset for {RequestName}.", typeof(TRequest).Name);
                });
    }

    /// <inheritdoc />
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        await _circuitBreakerPolicy.ExecuteAsync(() => next());
    }
}
