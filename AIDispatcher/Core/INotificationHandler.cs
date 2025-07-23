namespace AIDispatcher.Core;

/// <summary>
/// Antarmuka untuk handler notifikasi yang tidak mengembalikan hasil (void).
/// Digunakan pada pola publish/subscribe agar suatu notifikasi dapat diproses oleh satu atau banyak handler tanpa return value.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang ditangani.</typeparam>
public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Menangani notifikasi tanpa hasil (void).
    /// </summary>
    /// <param name="notification">Objek notifikasi yang akan diproses oleh handler.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses eksekusi handler.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}