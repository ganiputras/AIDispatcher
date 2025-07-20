namespace AIDispatcher.Dispatcher;

public class TimeoutOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}