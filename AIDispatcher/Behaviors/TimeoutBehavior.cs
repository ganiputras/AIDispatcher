using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Options;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Dispatcher behavior untuk menerapkan batas waktu (timeout) pada eksekusi request.
    /// Jika eksekusi request melebihi durasi yang dikonfigurasi, maka akan dilempar TimeoutException.
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class TimeoutBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private readonly TimeSpan _timeout;

        /// <summary>
        /// Membuat instance TimeoutBehavior dengan mengambil konfigurasi timeout dari options.
        /// </summary>
        /// <param name="options">Konfigurasi DispatcherOptions yang berisi DefaultTimeout.</param>
        public TimeoutBehavior(IOptions<DispatcherOptions> options)
        {
            _timeout = options.Value.DefaultTimeout;
        }

        /// <summary>
        /// Menangani eksekusi request dengan menerapkan timeout.
        /// Jika request melebihi waktu timeout, akan dilempar TimeoutException.
        /// </summary>
        /// <param name="request">Instance request yang diproses.</param>
        /// <param name="next">Delegate untuk mengeksekusi handler utama berikutnya di pipeline.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler utama jika selesai dalam batas waktu.</returns>
        /// <exception cref="TimeoutException">Dilempar jika waktu eksekusi request melebihi batas timeout.</exception>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Lewati logika timeout jika timeout disetel ke Infinite
            if (_timeout == Timeout.InfiniteTimeSpan)
                return await next(cancellationToken);

            using var timeoutCts = new CancellationTokenSource(_timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                return await next(linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"Request of type '{typeof(TRequest).Name}' timed out after {_timeout.TotalMilliseconds}ms.");
            }
        }
    }
}
