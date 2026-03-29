using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MW.Saga.MassTransit.Persistence;

namespace MW.Saga.MassTransit.Tests.Persistence;

public class TestPersistenceSaga : ISaga
{
    public Guid CorrelationId { get; set; }
}

public class SagaEfCorePersistenceHelperTests
{
    [Fact]
    public void UseEntityFrameworkCoreSagaRepository_Should_ThrowOnNullSagaRegistration()
    {
        var act = () => SagaEfCorePersistenceHelper
            .UseEntityFrameworkCoreSagaRepository<TestPersistenceSaga, DbContext>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("sagaRegistration");
    }
}
