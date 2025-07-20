using AIDispatcher.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Dispatcher;

public class DispatcherBuilder
{
    private readonly IServiceCollection _services;

    public DispatcherBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public DispatcherBuilder UseValidation()
    {
        _services.AddTransient(typeof(IDispatcherBehavior<,>), typeof(ValidationBehavior<,>));
        return this;
    }

    public DispatcherBuilder UseRetry(Action<RetryOptions>? configure = null)
    {
        if (configure != null)
        {
            var options = new RetryOptions();
            configure(options);
            _services.AddSingleton(options);
        }

        _services.AddTransient(typeof(IDispatcherBehavior<,>), typeof(RetryBehavior<,>));
        return this;
    }

    public DispatcherBuilder UseTimeout(TimeSpan timeout)
    {
        _services.AddSingleton(new TimeoutOptions { Timeout = timeout });
        _services.AddTransient(typeof(IDispatcherBehavior<,>), typeof(TimeoutBehavior<,>));
        return this;
    }
}