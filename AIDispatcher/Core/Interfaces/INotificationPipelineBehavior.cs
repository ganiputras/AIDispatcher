namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka untuk pipeline behavior pada notifikasi.
/// Digunakan untuk menyisipkan logika tambahan sebelum dan/atau sesudah handler notifikasi dijalankan.
/// </summary>
/// <typeparam name="TNotification">Tipe notifikasi yang akan diproses.</typeparam>
public interface INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Menangani proses sebelum dan sesudah handler notifikasi dijalankan.
    /// Dapat digunakan untuk menambah behavior seperti logging, exception handling, atau validasi pada pipeline notifikasi.
    /// </summary>
    /// <param name="notification">Objek notifikasi yang akan diproses.</param>
    /// <param name="handler">Delegasi ke handler notifikasi berikutnya dalam pipeline.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan eksekusi pipeline notifikasi.</returns>
    Task Handle(
        TNotification notification,
        NotificationHandlerDelegate handler,
        CancellationToken cancellationToken);
}