namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Menandakan request memiliki batas waktu eksekusi.
/// </summary>
public interface ITimeoutAware
{
    /// <summary>
    ///     Waktu maksimal eksekusi request sebelum dibatalkan.
    /// </summary>
    TimeSpan Timeout { get; }
}

//Contoh implementasi di request:
//public class MyRequest : IRequest<string>, ITimeoutAware
//{
//    public TimeSpan Timeout => TimeSpan.FromSeconds(2); // timeout 2 detik
//}