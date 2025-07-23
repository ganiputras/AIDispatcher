namespace AIDispatcher.Core.Commons;

/// <summary>
///     Menentukan batas waktu maksimal (dalam milidetik) untuk mengeksekusi sebuah handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class WithTimeoutAttribute : Attribute
{
    /// <summary>
    ///     Membuat atribut timeout dengan nilai tertentu.
    /// </summary>
    /// <param name="timeoutMilliseconds">Waktu maksimal dalam milidetik.</param>
    public WithTimeoutAttribute(int timeoutMilliseconds) => TimeoutMilliseconds = timeoutMilliseconds;

    /// <summary>
    ///     Durasi timeout dalam milidetik.
    /// </summary>
    public int TimeoutMilliseconds { get; }
}