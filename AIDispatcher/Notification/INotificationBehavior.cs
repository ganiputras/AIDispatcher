namespace AIDispatcher.Notification
{
    /// <summary>
    /// Pipeline behavior untuk notifikasi.
    /// Behavior ini dieksekusi sebelum dan sesudah handler notifikasi.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang diproses.</typeparam>
    public interface INotificationBehavior<TNotification>
        where TNotification : notnull
    {
        /// <summary>
        /// Menangani eksekusi behavior dan memanggil behavior berikutnya atau handler utama.
        /// </summary>
        /// <param name="notification">Notifikasi yang sedang diproses.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <param name="next">Delegate untuk melanjutkan eksekusi behavior berikutnya atau handler utama.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous.</returns>
        Task HandleAsync(
            TNotification notification,
            CancellationToken cancellationToken,
            Func<Task> next);
    }
}