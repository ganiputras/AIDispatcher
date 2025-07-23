namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Delegate untuk mengeksekusi handler akhir pada permintaan tanpa hasil (void/command) dalam pipeline dispatcher.
/// Digunakan untuk merepresentasikan eksekusi asynchronous handler berikutnya.
/// </summary>
/// <returns>Task asynchronous yang merepresentasikan proses handler akhir pada pipeline.</returns>
public delegate Task RequestHandlerDelegate();

/// <summary>
/// Delegate untuk mengeksekusi handler berikutnya dalam pipeline pada permintaan dengan response.
/// Digunakan untuk merepresentasikan eksekusi asynchronous handler berikutnya yang menghasilkan nilai response.
/// </summary>
/// <typeparam name="TResponse">Tipe respons yang dihasilkan handler.</typeparam>
/// <returns>Task asynchronous yang menghasilkan response dari handler akhir dalam pipeline.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();