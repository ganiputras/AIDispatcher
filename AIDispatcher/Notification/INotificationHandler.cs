namespace AIDispatcher.Notification
{
    /// <summary>
    /// Interface untuk menangani notifikasi berbasis event.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang akan ditangani.</typeparam>
    public interface INotificationHandler<in TNotification>
        where TNotification : notnull
    {
        /// <summary>
        /// Menangani notifikasi yang diterima secara asynchronous.
        /// </summary>
        /// <param name="notification">Instance notifikasi yang akan diproses.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous penanganan notifikasi.</returns>
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }
}