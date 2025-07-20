using AIDispatcher.Notification;

namespace AIDispatcher.Dispatcher;

/// <summary>
///     Titik masuk utama dispatcher untuk pemanggilan dari level root,
///     seperti dari <c>Program.cs</c>, background service, atau task scheduler.
///     Interface ini akan secara otomatis membuat scope baru untuk setiap pemanggilan,
///     sehingga cocok untuk skenario-skenario yang tidak berada dalam konteks DI scoped seperti controller.
/// </summary>
public interface IDispatcherRoot
{
    /// <summary>
    ///     Mengirim permintaan (request) ke handler melalui pipeline dan mengembalikan hasilnya.
    ///     Secara otomatis akan membuat service scope baru untuk menjamin lifetime yang tepat.
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang akan dikirim.</typeparam>
    /// <typeparam name="TResult">Tipe hasil/response dari request.</typeparam>
    /// <param name="request">Objek request yang ingin diproses.</param>
    /// <param name="cancellationToken">Token untuk pembatalan async (opsional).</param>
    /// <returns>Hasil dari handler request.</returns>
    Task<TResult> SendAsync<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : notnull;

    /// <summary>
    ///     Mempublikasikan notifikasi ke semua handler yang sesuai.
    ///     Sama seperti <see cref="INotificationDispatcher" />, namun dipanggil dari konteks luar (root) seperti background job.
    ///     Akan membuat scope baru untuk setiap eksekusi.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi.</typeparam>
    /// <param name="notification">Objek notifikasi yang akan diproses.</param>
    /// <param name="cancellationToken">Token pembatalan (opsional).</param>
    Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}