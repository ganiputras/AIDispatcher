using AIDispatcher.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.PrePostProcessor;

public static class PrePostProcessorServiceCollectionExtensions
{
    public static IServiceCollection AddPrePostProcessorBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IDispatcherBehavior<,>), typeof(PrePostProcessorBehavior<,>));
        return services;
    }
}