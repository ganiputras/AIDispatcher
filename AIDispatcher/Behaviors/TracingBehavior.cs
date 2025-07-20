using AIDispatcher.Dispatcher;
using AIDispatcher.Observability;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Menambahkan dukungan distributed tracing pada pipeline dispatcher menggunakan ActivitySource.
    /// Kompatibel dengan OpenTelemetry, Jaeger, Zipkin, atau sistem tracing lainnya.
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class TracingBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private static readonly ActivitySource ActivitySource = DispatcherActivitySource.Instance;
        private readonly ILogger<TracingBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Membuat instance TracingBehavior dengan dependency logger.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat log debug dan error terkait tracing.</param>
        public TracingBehavior(ILogger<TracingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Menangani eksekusi request dengan membuat dan mengelola Activity tracing.
        /// Merekam metadata request, status keberhasilan, dan exception jika terjadi.
        /// </summary>
        /// <param name="request">Instance request yang diproses.</param>
        /// <param name="next">Delegate untuk mengeksekusi handler utama berikutnya di pipeline.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler utama.</returns>
        /// <exception cref="Exception">Melempar ulang exception jika terjadi kegagalan saat mengeksekusi handler.</exception>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            // Mulai activity tracing untuk request ini
            using var activity = ActivitySource.StartActivity($"Dispatcher.{requestName}");

            if (activity != null)
            {
                activity.SetTag("dispatcher.request_type", requestName);
                activity.SetTag("dispatcher.request", request.ToString());
            }
            _logger.LogDebug("Tracing started for {RequestType}", requestName);

            try
            {
                var response = await next(cancellationToken);

                if (activity != null) activity.SetTag("dispatcher.success", true);

                return response;
            }
            catch (Exception ex)
            {
                if (activity != null)
                {
                    activity.SetTag("dispatcher.success", false);
                    activity.SetTag("dispatcher.exception", ex.Message);
                    activity.RecordException(ex);
                }
                _logger.LogError(ex, "Tracing failure for {RequestType}", requestName);

                throw;
            }
        }
    }
}
