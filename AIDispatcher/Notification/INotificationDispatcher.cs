namespace AIDispatcher.Notification
{
    /// <summary>
    /// Interface untuk mendispatch notifikasi ke handler-handler yang terdaftar.
    /// </summary>
    public interface INotificationDispatcher
    {
        /// <summary>
        /// Mempublikasikan notifikasi ke semua handler yang sesuai secara asynchronous.
        /// </summary>
        /// <typeparam name="TNotification">Tipe notifikasi yang akan dipublikasikan.</typeparam>
        /// <param name="notification">Instance notifikasi yang akan dikirim.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous publikasi notifikasi.</returns>
        Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : notnull;
    }
}