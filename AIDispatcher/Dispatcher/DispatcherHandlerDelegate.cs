namespace AIDispatcher.Dispatcher;


/// <summary>
///     Delegate untuk menangani eksekusi lanjutan dalam pipeline dispatcher.
///     <para>
///         Digunakan oleh setiap <see cref="IDispatcherBehavior{TRequest, TResponse}" /> untuk memanggil langkah selanjutnya dalam pipeline.
///     </para>
/// </summary>
/// <typeparam name="TResponse">Tipe hasil yang dikembalikan oleh handler.</typeparam>
/// <returns>Task yang menghasilkan <typeparamref name="TResponse" />.</returns>

public delegate Task<TResponse> DispatcherHandlerDelegate<TResponse>(CancellationToken cancellationToken);