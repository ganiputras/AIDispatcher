namespace AIDispatcher.Core.Commons;

/// <summary>
/// Menentukan batas waktu maksimal (timeout) dalam milidetik untuk mengeksekusi sebuah handler.
/// Digunakan sebagai attribute pada class handler untuk mengatur durasi timeout secara spesifik.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class WithTimeoutAttribute : Attribute
{
    /// <summary>
    /// Membuat atribut timeout dengan nilai waktu maksimal tertentu.
    /// </summary>
    /// <param name="timeoutMilliseconds">Durasi timeout maksimum eksekusi handler, dalam milidetik.</param>
    public WithTimeoutAttribute(int timeoutMilliseconds)
    {
        TimeoutMilliseconds = timeoutMilliseconds;
    }

    /// <summary>
    /// Durasi timeout maksimal dalam milidetik.
    /// Jika handler melebihi waktu ini, eksekusi akan dibatalkan.
    /// </summary>
    public int TimeoutMilliseconds { get; }
}