namespace AIDispatcher.Core;

/// <summary>
///     Menangani notifikasi yang tidak mengembalikan hasil.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang ditangani.</typeparam>
public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    /// <summary>
    ///     Menangani notifikasi tanpa hasil.
    /// </summary>
    /// <param name="notification">Objek notifikasi.</param>
    /// <param name="cancellationToken">Token untuk pembatalan operasi.</param>
    /// <returns>Task yang merepresentasikan eksekusi handler.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}