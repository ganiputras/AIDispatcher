using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Dispatcher
{
    /// <summary>
    ///     Implementasi default dari <see cref="IDispatcher" />.
    ///     Menyusun dan mengeksekusi pipeline behavior sebelum memanggil handler yang sesuai.
    /// </summary>
    /// <remarks>
    ///     Class ini mengambil handler dan behavior dari dependency injection (DI), lalu menyusunnya
    ///     menjadi rantai eksekusi secara berurutan. Setiap behavior membungkus eksekusi berikutnya
    ///     untuk memungkinkan penerapan concern lintas (cross-cutting) seperti logging, validasi, retry, dll.
    /// </remarks>
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///     Inisialisasi instance baru dari <see cref="Dispatcher" />.
        /// </summary>
        /// <param name="serviceProvider">
        ///     <see cref="IServiceProvider" /> yang digunakan untuk resolve handler dan behavior dari DI container.
        /// </param>
        public Dispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     Mengirim permintaan melalui pipeline dan mengembalikan respons dari handler yang sesuai.
        /// </summary>
        /// <typeparam name="TRequest">Tipe dari permintaan.</typeparam>
        /// <typeparam name="TResponse">Tipe dari respons.</typeparam>
        /// <param name="request">Permintaan yang akan diproses.</param>
        /// <param name="cancellationToken">Token pembatalan untuk menghentikan eksekusi secara kooperatif.</param>
        /// <returns>Objek respons dari handler.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Dilempar jika tidak ada handler yang terdaftar untuk tipe <typeparamref name="TRequest" />.
        /// </exception>
        public async Task<TResponse> SendAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default)
        {
            // Ambil handler dan semua behavior yang relevan dari container.
            var handler = _serviceProvider.GetRequiredService<IDispatcherHandler<TRequest, TResponse>>();
            var behaviors = _serviceProvider
                .GetServices<IDispatcherBehavior<TRequest, TResponse>>()
                .ToList();

            // Delegate utama yang menjalankan handler secara langsung.
            DispatcherHandlerDelegate<TResponse> handlerDelegate = ct => handler.HandleAsync(request, ct);

            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = handlerDelegate;
                handlerDelegate = ct => behavior.HandleAsync(request, next, ct);
            }

            // Jalankan seluruh rantai eksekusi pipeline.
            return await handlerDelegate(cancellationToken);

        }
    }
}
