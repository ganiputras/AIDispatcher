namespace AIDispatcher.Dispatcher;

/// <summary>
///     Delegate yang mewakili fungsi handler berikutnya dalam pipeline.
///     Digunakan untuk meneruskan eksekusi ke behavior selanjutnya atau handler akhir.
/// </summary>
/// <typeparam name="TResponse">Tipe respons yang dikembalikan.</typeparam>
public delegate Task<TResponse> DispatcherHandlerDelegate<TResponse>();