namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Antarmuka pipeline behavior untuk notifikasi.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi.</typeparam>
public interface INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    /// <summary>
    ///     Menangani proses sebelum dan sesudah handler notifikasi dijalankan.
    /// </summary>
    /// <param name="notification">Objek notifikasi.</param>
    /// <param name="handler">Delegasi ke handler notifikasi.</param>
    /// <param name="cancellationToken">Token pembatalan.</param>
    Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken);
}