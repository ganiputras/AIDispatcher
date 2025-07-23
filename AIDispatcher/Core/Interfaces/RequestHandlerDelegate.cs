namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Delegasi untuk eksekusi handler akhir pada permintaan tanpa hasil.
/// </summary>
public delegate Task RequestHandlerDelegate();

/// <summary>
///     Delegasi untuk mengeksekusi handler berikutnya dalam pipeline.
/// </summary>
/// <typeparam name="TResponse">Tipe respons.</typeparam>
/// <returns>Hasil dari handler akhir.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();