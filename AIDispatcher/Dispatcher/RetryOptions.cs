namespace AIDispatcher.Dispatcher;

public class RetryOptions
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(200);
}