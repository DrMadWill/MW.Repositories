using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace MW.Saga.MassTransit.Persistence;

/// <summary>
/// Reusable helper for registering EF Core-based persistence for saga state storage.
/// Supports generic <typeparamref name="TDbContext"/> and keeps the design aligned
/// with the platform's persistence approach.
/// </summary>
public static class SagaEfCorePersistenceHelper
{
    /// <summary>
    /// Configures EF Core saga repository for the given saga state type.
    /// </summary>
    /// <typeparam name="TSaga">The saga state type.</typeparam>
    /// <typeparam name="TDbContext">The EF Core DbContext type.</typeparam>
    /// <param name="sagaRegistration">The saga registration configurator.</param>
    public static void UseEntityFrameworkCoreSagaRepository<TSaga, TDbContext>(
        ISagaRegistrationConfigurator<TSaga> sagaRegistration)
        where TSaga : class, ISaga
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(sagaRegistration);

        sagaRegistration.EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<TDbContext>();
        });
    }
}
