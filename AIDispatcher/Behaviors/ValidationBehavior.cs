using AIDispatcher.Dispatcher;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AIDispatcher.Behaviors
{
    /// <summary>
    /// Behavior pipeline untuk validasi menggunakan FluentValidation.
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang divalidasi.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan.</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Membuat instance ValidationBehavior dengan dependency logger dan daftar validator.
        /// </summary>
        /// <param name="logger">Logger untuk mencatat peringatan validasi.</param>
        /// <param name="validators">Koleksi validator untuk request.</param>
        public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger,
            IEnumerable<IValidator<TRequest>> validators)
        {
            _logger = logger;
            _validators = validators;
        }

        /// <summary>
        /// Menangani eksekusi request dengan validasi menggunakan FluentValidation sebelum memanggil handler berikutnya.
        /// Jika ada error validasi, melempar ValidationException.
        /// </summary>
        /// <param name="request">Instance request yang akan divalidasi.</param>
        /// <param name="next">Delegate untuk mengeksekusi handler utama berikutnya.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler utama jika validasi sukses.</returns>
        /// <exception cref="ValidationException">Dilempar jika validasi gagal.</exception>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Jika ada validator yang terdaftar, jalankan validasi
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // Jalankan semua validator secara paralel dan kumpulkan error
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

            // Lanjutkan ke handler berikutnya jika validasi sukses
            return await next(cancellationToken);
        }
    }
}
