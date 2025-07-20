using AIDispatcher.Dispatcher;
using AIDispatcher.SampleApp.Handlers;
using Microsoft.Extensions.Hosting;

namespace AIDispatcher.SampleApp;

public class MyBackgroundService : BackgroundService
{
    private readonly IDispatcherRoot _dispatcherRoot;

    public MyBackgroundService(IDispatcherRoot dispatcherRoot)
    {
        _dispatcherRoot = dispatcherRoot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _dispatcherRoot.SendAsync<CreateUserCommand, object>(new CreateUserCommand("Test", "Test@domain.com"), stoppingToken);
    }
}

public class MyBackgroundService2 : BackgroundService
{
    private readonly IDispatcher _dispatcherRoot;

    public MyBackgroundService2(IDispatcher dispatcherRoot)
    {
        _dispatcherRoot = dispatcherRoot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _dispatcherRoot.SendAsync<CreateUserCommand, object>(new CreateUserCommand("Test", "Test@domain.com"), stoppingToken);
    }
}