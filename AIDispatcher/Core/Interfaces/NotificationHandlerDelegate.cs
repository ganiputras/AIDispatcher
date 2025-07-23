namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Delegate untuk handler notifikasi dalam pipeline dispatcher.
/// Digunakan untuk merepresentasikan metode asynchronous yang menjalankan handler notifikasi berikutnya.
/// </summary>
/// <returns>Task asynchronous yang merepresentasikan proses eksekusi handler notifikasi.</returns>
public delegate Task NotificationHandlerDelegate();