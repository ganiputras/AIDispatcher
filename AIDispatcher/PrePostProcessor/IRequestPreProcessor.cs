namespace AIDispatcher.PrePostProcessor;

/// <summary>
///     Interface for pre-processing logic before the main handler executes.
/// </summary>
public interface IRequestPreProcessor<in TRequest>
{
    Task ProcessAsync(TRequest request, CancellationToken cancellationToken);
}