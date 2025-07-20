using AIDispatcher.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Dispatcher;
/// <summary>
///     Implementasi dari <see cref="IDispatcherRoot"/> yang menggunakan <c>scoped</c> lifetime service.
///     Class ini memastikan bahwa setiap pengiriman request atau notifikasi dilakukan dalam scope terpisah.
///     Hal ini berguna untuk isolasi dependency, seperti konteks database, per permintaan.
/// </summary>
public class DispatcherRoot : IDispatcherRoot
{
    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Konstruktor <see cref="DispatcherRoot"/>.
    /// </summary>
    /// <param name="provider">
    ///     Penyedia layanan utama untuk membuat scope baru dan me-resolve dispatcher.
    /// </param>
    public DispatcherRoot(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    ///     Mengirim request ke handler yang sesuai dan mengembalikan hasilnya.
    ///     Setiap pemanggilan akan dibuatkan scope baru untuk memastikan isolasi dependency.
    /// </summary>
    /// <typeparam name="TRequest">Tipe objek request.</typeparam>
    /// <typeparam name="TResult">Tipe hasil yang dikembalikan oleh handler.</typeparam>
    /// <param name="request">Objek request yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan eksekusi request.</param>
    /// <returns>Hasil pemrosesan dari handler.</returns>
    public async Task<TResult> SendAsync<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : notnull
    {
        using var scope = _provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        return await dispatcher.SendAsync<TRequest, TResult>(request, cancellationToken);
    }

    /// <summary>
    ///     Mempublikasikan notifikasi ke semua handler yang relevan.
    ///     Setiap pemanggilan akan dibuatkan scope baru agar semua dependency per notifikasi tetap terisolasi.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi yang akan dikirim.</typeparam>
    /// <param name="notification">Objek notifikasi.</param>
    /// <param name="cancellationToken">Token untuk membatalkan eksekusi notifikasi.</param>
    public async Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        using var scope = _provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<INotificationDispatcher>();
        await dispatcher.PublishAsync(notification, cancellationToken);
    }
}
