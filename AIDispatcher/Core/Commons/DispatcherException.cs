namespace AIDispatcher.Core.Commons;

/// <summary>
///     Exception khusus untuk kesalahan internal dalam Dispatcher.
/// </summary>
public class DispatcherException : Exception
{
    public DispatcherException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}