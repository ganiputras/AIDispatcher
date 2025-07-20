using AIDispatcher.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.PrePostProcessor;

/// <summary>
///     Behavior that executes registered pre- and post-processors around the main request handler.
/// </summary>
public class PrePostProcessorBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;
    private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;

    public PrePostProcessorBehavior(IServiceProvider serviceProvider)
    {
        _preProcessors = serviceProvider.GetServices<IRequestPreProcessor<TRequest>>();
        _postProcessors = serviceProvider.GetServices<IRequestPostProcessor<TRequest, TResponse>>();
    }

    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken,
        DispatcherHandlerDelegate<TResponse> next)
    {
        foreach (var pre in _preProcessors)
            await pre.ProcessAsync(request, cancellationToken);

        var response = await next();

        foreach (var post in _postProcessors)
            await post.ProcessAsync(request, response, cancellationToken);

        return response;
    }
}