using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Behavior untuk mengulangi eksekusi handler saat terjadi exception (retry).
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class RetryBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;
        private readonly int _maxRetries;

        /// <summary>
        /// Membuat instance RetryBehavior dengan dependency logger dan jumlah maksimal percobaan ulang.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat peringatan saat retry.</param>
        /// <param name="maxRetries">Jumlah maksimal percobaan ulang (default 3).</param>
        public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger, int maxRetries = 3)
        {
            _logger = logger;
            _maxRetries = maxRetries;
        }

        /// <summary>
        /// Menangani eksekusi request dengan mencoba ulang jika terjadi exception hingga maksimal percobaan.
        /// </summary>
        /// <param name="request">Instance request yang diproses.</param>
        /// <param name="next">Delegate untuk mengeksekusi handler utama berikutnya di pipeline.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler utama jika berhasil.</returns>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            for (var attempt = 1; ; attempt++)
                try
                {
                    return await next(cancellationToken);
                }
                catch (Exception ex) when (attempt <= _maxRetries)
                {
                    _logger.LogWarning(ex, "Retry {Attempt} for {RequestType}", attempt, typeof(TRequest).Name);
                    await Task.Delay(100 * attempt, cancellationToken);
                }
        }
    }
}
