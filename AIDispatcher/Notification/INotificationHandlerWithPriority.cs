namespace AIDispatcher.Notification
{
    /// <summary>
    /// Interface yang menandakan handler notifikasi memiliki prioritas.
    /// Prioritas ini digunakan untuk menentukan urutan eksekusi handler.
    /// </summary>
    public interface INotificationHandlerWithPriority
    {
        /// <summary>
        /// Mendapatkan nilai prioritas handler.
        /// Handler dengan nilai prioritas lebih tinggi akan dijalankan lebih dulu.
        /// </summary>
        int Priority { get; }
    }
}