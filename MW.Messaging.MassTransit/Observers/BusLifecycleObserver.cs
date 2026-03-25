using MassTransit;
using Microsoft.Extensions.Logging;

namespace MW.Messaging.MassTransit.Observers;

public class BusLifecycleObserver : IBusObserver
{
    private readonly ILogger<BusLifecycleObserver> _logger;

    public BusLifecycleObserver(ILogger<BusLifecycleObserver> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void PostCreate(IBus bus)
    {
        _logger.LogInformation("MassTransit bus created");
    }

    public void CreateFaulted(Exception exception)
    {
        _logger.LogError(exception, "MassTransit bus creation faulted");
    }

    public Task PreStart(IBus bus)
    {
        _logger.LogInformation("MassTransit bus starting");
        return Task.CompletedTask;
    }

    public Task PostStart(IBus bus, Task<BusReady> busReady)
    {
        _logger.LogInformation("MassTransit bus started and ready");
        return Task.CompletedTask;
    }

    public Task StartFaulted(IBus bus, Exception exception)
    {
        _logger.LogError(exception, "MassTransit bus start faulted");
        return Task.CompletedTask;
    }

    public Task PreStop(IBus bus)
    {
        _logger.LogInformation("MassTransit bus stopping");
        return Task.CompletedTask;
    }

    public Task PostStop(IBus bus)
    {
        _logger.LogInformation("MassTransit bus stopped");
        return Task.CompletedTask;
    }

    public Task StopFaulted(IBus bus, Exception exception)
    {
        _logger.LogError(exception, "MassTransit bus stop faulted");
        return Task.CompletedTask;
    }
}
