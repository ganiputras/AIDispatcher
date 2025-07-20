namespace AIDispatcher.Notification
{
    /// <summary>
    /// Executor pipeline notifikasi yang menjalankan serangkaian <see cref="INotificationBehavior{TNotification}"/>
    /// secara berurutan, dan pada akhirnya mengeksekusi handler utama (final handler).
    /// </summary>
    public static class NotificationPipelineExecutor
    {
        /// <summary>
        /// Menjalankan pipeline notifikasi dengan memanggil semua <see cref="INotificationBehavior{TNotification}"/>
        /// dalam urutan terbalik, lalu mengeksekusi handler utama.
        /// </summary>
        /// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
        /// <param name="notification">Instance notifikasi yang sedang diproses.</param>
        /// <param name="behaviors">Koleksi pipeline behavior yang akan dijalankan.</param>
        /// <param name="finalHandler">Delegate yang merepresentasikan handler utama yang akan dipanggil setelah semua behavior selesai.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan eksekusi asynchronous pipeline notifikasi.</returns>
        public static async Task ExecuteAsync<TNotification>(
            TNotification notification,
            IEnumerable<INotificationBehavior<TNotification>> behaviors,
            Func<Task> finalHandler,
            CancellationToken cancellationToken)
            where TNotification : notnull
        {
            var enumerator = behaviors.Reverse().GetEnumerator();

            Task Handler()
            {
                if (!enumerator.MoveNext())
                    return finalHandler();

                return enumerator.Current.HandleAsync(notification, cancellationToken, Handler);
            }

            await Handler();
        }
    }
}