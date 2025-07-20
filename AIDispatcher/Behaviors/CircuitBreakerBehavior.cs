using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Behavior untuk memblokir request jika jumlah error melebihi ambang batas tertentu (circuit breaker).
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class CircuitBreakerBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private static readonly ConcurrentDictionary<string, int> FailureCounts = new();
        private readonly int _failureThreshold;
        private readonly ILogger<CircuitBreakerBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Membuat instance CircuitBreakerBehavior dengan logger dan ambang batas kegagalan.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat aktivitas dan error.</param>
        /// <param name="failureThreshold">Ambang batas jumlah kegagalan sebelum memblokir request (default 3).</param>
        public CircuitBreakerBehavior(
            ILogger<CircuitBreakerBehavior<TRequest, TResponse>> logger,
            int failureThreshold = 3)
        {
            _logger = logger;
            _failureThreshold = failureThreshold;
        }

        /// <summary>
        /// Menangani eksekusi request dengan mekanisme circuit breaker.
        /// Jika jumlah kegagalan melebihi ambang batas, request akan diblokir dengan melempar exception.
        /// </summary>
        /// <param name="request">Instance request yang diproses.</param>
        /// <param name="next">Delegate untuk mengeksekusi handler utama berikutnya di pipeline.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler utama jika tidak diblokir.</returns>
        /// <exception cref="InvalidOperationException">Dilempar jika circuit breaker sedang terbuka (membatasi request).</exception>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var key = typeof(TRequest).FullName ?? typeof(TRequest).Name;

            if (FailureCounts.TryGetValue(key, out var count) && count >= _failureThreshold)
            {
                _logger.LogError("Circuit breaker open for {RequestType}", key);
                throw new InvalidOperationException($"Circuit breaker open for {key}");
            }

            try
            {
                var response = await next(cancellationToken);
                FailureCounts.TryRemove(key, out _);
                return response;
            }
            catch (Exception ex)
            {
                FailureCounts.AddOrUpdate(key, 1, (_, current) => current + 1);
                _logger.LogWarning(ex, "Failure #{Count} for {RequestType}", FailureCounts[key], key); throw;
            }
        }
    }
}
