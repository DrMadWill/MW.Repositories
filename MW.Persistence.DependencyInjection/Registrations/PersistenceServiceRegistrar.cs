using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Transactions;
using MW.Persistence.Abstractions.UnitOfWork;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.Transactions;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;

namespace MW.Persistence.DependencyInjection.Registrations;

/// <summary>
/// Contains explicit service registrations for persistence-related abstractions and their EF Core implementations.
/// All registrations use scoped lifetime to align with DbContext lifecycle.
/// </summary>
internal static class PersistenceServiceRegistrar
{
    /// <summary>
    /// Registers all persistence services (repositories, UnitOfWork, transaction manager) with scoped lifetime.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext to resolve.</typeparam>
    /// <param name="services">The service collection to register services into.</param>
    internal static void RegisterServices<TDbContext>(IServiceCollection services)
        where TDbContext : DbContext
    {
        RegisterRepositories<TDbContext>(services);
        RegisterUnitOfWork<TDbContext>(services);
        RegisterTransactionManager<TDbContext>(services);
    }

    /// <summary>
    /// Registers generic repository implementations with scoped lifetime.
    /// Maps each repository abstraction to its EF Core implementation, resolving DbContext as the concrete TDbContext type.
    /// </summary>
    private static void RegisterRepositories<TDbContext>(IServiceCollection services)
        where TDbContext : DbContext
    {
        // IReadRepository<TEntity, TId> → EfReadRepository<TEntity, TId>
        services.AddScoped(typeof(IReadRepository<,>), typeof(EfReadRepository<,>));

        // IWriteRepository<TEntity, TId> → EfWriteRepository<TEntity, TId>
        services.AddScoped(typeof(IWriteRepository<,>), typeof(EfWriteRepository<,>));

        // IRepository<TEntity, TId> → EfRepository<TEntity, TId>
        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

        // IAggregateRepository<TAggregate, TId> → EfAggregateRepository<TAggregate, TId>
        services.AddScoped(typeof(IAggregateRepository<,>), typeof(EfAggregateRepository<,>));

        // IProjectionReadRepository<TEntity, TId> → EfProjectionReadRepository<TEntity, TId>
        services.AddScoped(typeof(IProjectionReadRepository<,>), typeof(EfProjectionReadRepository<,>));
    }

    /// <summary>
    /// Registers the Unit of Work implementation with scoped lifetime.
    /// UnitOfWork must share the same DbContext scope as repositories.
    /// </summary>
    private static void RegisterUnitOfWork<TDbContext>(IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IUnitOfWork>(sp =>
        {
            var dbContext = sp.GetRequiredService<TDbContext>();
            return new EfUnitOfWork(dbContext);
        });
    }

    /// <summary>
    /// Registers the transaction manager implementation with scoped lifetime.
    /// Transaction manager must share the same DbContext scope as repositories and UnitOfWork.
    /// </summary>
    private static void RegisterTransactionManager<TDbContext>(IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<ITransactionManager>(sp =>
        {
            var dbContext = sp.GetRequiredService<TDbContext>();
            return new EfTransactionManager(dbContext);
        });
    }
}
