using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Mengirimkan metrik untuk request yang didispatch, kompatibel dengan OpenTelemetry Metrics (misalnya Prometheus/Grafana).
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class DispatcherMetricsBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private static readonly Meter Meter = new("AIDispatcher.Metrics");
        private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("dispatcher_requests_total");

        private static readonly Histogram<double> DurationHistogram =
            Meter.CreateHistogram<double>("dispatcher_duration_ms");

        private readonly ILogger<DispatcherMetricsBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Membuat instance DispatcherMetricsBehavior dengan dependency logger.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat aktivitas debug terkait metrik.</param>
        public DispatcherMetricsBehavior(ILogger<DispatcherMetricsBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Menangani eksekusi request dengan mencatat metrik jumlah request dan durasi eksekusi.
        /// Metrik ini kompatibel dengan sistem monitoring OpenTelemetry.
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
            var startTime = DateTime.UtcNow;

            _logger.LogDebug("Metrics started for {RequestType}", requestName);

            try
            {
                var response = await next(cancellationToken);

                RequestCounter.Add(1, KeyValuePair.Create<string, object?>("request_type", requestName));
                DurationHistogram.Record((DateTime.UtcNow - startTime).TotalMilliseconds,
                    KeyValuePair.Create<string, object?>("request_type", requestName));

                return response;
            }
            catch (Exception ex)
            {
                RequestCounter.Add(1, new KeyValuePair<string, object?>[]
                {
                    new("request_type", requestName),
                    new("exception", ex.GetType().Name)
                });

                throw;
            }
        }
    }
}
