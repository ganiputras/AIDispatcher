namespace AIDispatcher.Dispatcher;

/// <summary>
///     Represents an unhandled exception that occurs during the dispatcher pipeline execution.
///     Used for consistent error logging, telemetry tagging, and centralized diagnostics.
/// </summary>
public class DispatcherUnhandledException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DispatcherUnhandledException" /> class.
    /// </summary>
    /// <param name="requestType">The type of the request that triggered the exception.</param>
    /// <param name="innerException">The actual exception thrown by the handler or behavior.</param>
    public DispatcherUnhandledException(Type requestType, Exception innerException)
        : base($"Unhandled exception during processing request of type '{requestType.Name}'", innerException)
    {
        RequestType = requestType;
    }

    /// <summary>
    ///     The request type that caused the exception.
    /// </summary>
    public Type RequestType { get; }
}