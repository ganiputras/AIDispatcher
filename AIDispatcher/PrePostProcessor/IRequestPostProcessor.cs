namespace AIDispatcher.PrePostProcessor;

/// <summary>
///     Interface for post-processing logic after the main handler executes.
/// </summary>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    Task ProcessAsync(TRequest request, TResponse response, CancellationToken cancellationToken);
}