using FluentAssertions;
using MassTransit;
using MW.Messaging.MassTransit.Observers;
using Microsoft.Extensions.Logging;
using Moq;

namespace MW.Messaging.MassTransit.Tests.Observers;

public class BusLifecycleObserverTests
{
    private readonly Mock<ILogger<BusLifecycleObserver>> _logger = new();
    private readonly Mock<IBus> _bus = new();

    [Fact]
    public void Constructor_Should_ThrowOnNullLogger()
    {
        var act = () => new BusLifecycleObserver(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Should_CreateWithLogger()
    {
        var logger = Mock.Of<ILogger<BusLifecycleObserver>>();

        var observer = new BusLifecycleObserver(logger);

        observer.Should().NotBeNull();
        observer.Should().BeAssignableTo<global::MassTransit.IBusObserver>();
    }

    [Fact]
    public void PostCreate_Should_LogInformation()
    {
        var observer = new BusLifecycleObserver(_logger.Object);

        observer.PostCreate(_bus.Object);

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void CreateFaulted_Should_LogError()
    {
        var observer = new BusLifecycleObserver(_logger.Object);
        var exception = new InvalidOperationException("creation failed");

        observer.CreateFaulted(exception);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PreStart_Should_LogInformation()
    {
        var observer = new BusLifecycleObserver(_logger.Object);

        await observer.PreStart(_bus.Object);

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PostStart_Should_LogInformation()
    {
        var observer = new BusLifecycleObserver(_logger.Object);

        await observer.PostStart(_bus.Object, Task.FromResult(Mock.Of<BusReady>()));

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StartFaulted_Should_LogError()
    {
        var observer = new BusLifecycleObserver(_logger.Object);
        var exception = new InvalidOperationException("start failed");

        await observer.StartFaulted(_bus.Object, exception);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PreStop_Should_LogInformation()
    {
        var observer = new BusLifecycleObserver(_logger.Object);

        await observer.PreStop(_bus.Object);

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PostStop_Should_LogInformation()
    {
        var observer = new BusLifecycleObserver(_logger.Object);

        await observer.PostStop(_bus.Object);

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StopFaulted_Should_LogError()
    {
        var observer = new BusLifecycleObserver(_logger.Object);
        var exception = new InvalidOperationException("stop failed");

        await observer.StopFaulted(_bus.Object, exception);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
