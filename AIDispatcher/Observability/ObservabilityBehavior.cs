using AIDispatcher.Dispatcher;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIDispatcher.Observability
{
    /// <summary>
    /// Menambahkan tracing OpenTelemetry dan logging di sekitar pipeline permintaan (request pipeline).
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class ObservabilityBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private static readonly ActivitySource ActivitySource = new("AIDispatcher");
        private readonly ILogger<ObservabilityBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Membuat instance ObservabilityBehavior dengan dependency logger.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat aktivitas dan error.</param>
        public ObservabilityBehavior(ILogger<ObservabilityBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Menangani eksekusi request dengan menambahkan tracing OpenTelemetry dan logging.
        /// Membuat aktivitas tracing baru yang mencakup eksekusi request dan mencatat hasilnya.
        /// </summary>
        /// <param name="request">Instance request yang sedang diproses.</param>
        /// <param name="next">Delegate untuk melanjutkan ke handler berikutnya di pipeline.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler berikutnya.</returns>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            using var activity = ActivitySource.StartActivity($"AIDispatcher: {requestName}");

            _logger.LogInformation("Handling request {RequestType}", requestName);

            activity?.SetTag("request.type", requestName);
            activity?.SetTag("dispatcher.layer", "AIDispatcher");

            try
            {
                var response = await next(cancellationToken);

                _logger.LogInformation("Successfully handled request {RequestType}", requestName);
                activity?.SetTag("request.success", true);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling request {RequestType}", requestName);
                activity?.SetTag("request.success", false);
                activity?.SetTag("exception", ex.Message);
                throw;
            }
        }
    }
}
