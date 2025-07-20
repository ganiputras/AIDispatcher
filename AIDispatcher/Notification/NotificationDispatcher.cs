using AIDispatcher.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AIDispatcher.Notification
{
    /// <summary>
    /// Implementasi default NotificationDispatcher.
    /// Mendukung handler paralel, prioritas, dan pipeline.
    /// </summary>
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DispatcherOptions _options;

        /// <summary>
        /// Membuat instance NotificationDispatcher dengan service provider dan opsi konfigurasi dispatcher.
        /// </summary>
        /// <param name="serviceProvider">Service provider untuk scope dan resolve dependency.</param>
        /// <param name="options">Opsi konfigurasi dispatcher yang diinject dari IOptions.</param>
        public NotificationDispatcher(IServiceProvider serviceProvider, IOptions<DispatcherOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        /// <summary>
        /// Mempublikasikan notifikasi ke semua handler yang terdaftar.
        /// Handler dapat dijalankan secara paralel atau berurutan, dan melalui pipeline behavior.
        /// </summary>
        /// <typeparam name="TNotification">Tipe notifikasi yang dikirimkan.</typeparam>
        /// <param name="notification">Instance notifikasi yang akan dipublikasikan.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous.</returns>
        public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : notnull
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var handlers = services.GetServices<INotificationHandler<TNotification>>().ToList();
            var behaviors = services.GetServices<INotificationBehavior<TNotification>>().ToList();

            if (!handlers.Any())
                return;

            if (_options.NotificationHandlerPriorityEnabled)
                handlers = ApplyPriority(handlers);

            if (_options.ParallelNotificationHandlers)
            {
                var tasks = handlers.Select(handler =>
                    ExecutePipeline(notification, handler, behaviors, cancellationToken));
                await Task.WhenAll(tasks);
            }
            else
            {
                foreach (var handler in handlers)
                    await ExecutePipeline(notification, handler, behaviors, cancellationToken);
            }
        }

        /// <summary>
        /// Menerapkan pengurutan handler berdasarkan prioritas.
        /// Handler dengan prioritas lebih tinggi akan dieksekusi lebih dulu.
        /// </summary>
        /// <typeparam name="TNotification">Tipe notifikasi.</typeparam>
        /// <param name="handlers">Daftar handler yang akan diurutkan.</param>
        /// <returns>Daftar handler yang sudah diurutkan berdasarkan prioritas.</returns>
        private static List<INotificationHandler<TNotification>> ApplyPriority<TNotification>(
            List<INotificationHandler<TNotification>> handlers)
            where TNotification : notnull
        {
            return handlers
                .OrderByDescending(h => (h as INotificationHandlerWithPriority)?.Priority ?? 0)
                .ToList();
        }

        /// <summary>
        /// Mengeksekusi pipeline behavior sebelum dan sesudah pemanggilan handler notifikasi.
        /// </summary>
        /// <typeparam name="TNotification">Tipe notifikasi.</typeparam>
        /// <param name="notification">Notifikasi yang akan diproses.</param>
        /// <param name="handler">Handler yang akan menangani notifikasi.</param>
        /// <param name="behaviors">Daftar pipeline behavior yang akan dijalankan.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous eksekusi pipeline.</returns>
        private static Task ExecutePipeline<TNotification>(
            TNotification notification,
            INotificationHandler<TNotification> handler,
            List<INotificationBehavior<TNotification>> behaviors,
            CancellationToken cancellationToken)
            where TNotification : notnull
        {
            return NotificationPipelineExecutor.ExecuteAsync(
                notification,
                behaviors,
                () => handler.Handle(notification, cancellationToken),
                cancellationToken
            );
        }
    }
}
