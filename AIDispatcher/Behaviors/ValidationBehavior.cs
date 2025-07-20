using AIDispatcher.Dispatcher;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Behaviors;

/// <summary>
/// Behavior pipeline untuk validasi menggunakan FluentValidation.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger, IEnumerable<IValidator<TRequest>> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken, DispatcherHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning("Validation failed for request {RequestType}: {Failures}", typeof(TRequest).Name, failures);
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}