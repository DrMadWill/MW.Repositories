using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Transactions;
using MW.Persistence.Abstractions.UnitOfWork;
using MW.Persistence.DependencyInjection.Extensions;
using MW.Persistence.DependencyInjection.Options;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.Transactions;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.DependencyInjection;

/// <summary>
/// PTST-033: Integration tests for dependency injection registration.
/// Validates all services are registered with correct implementations and lifetimes.
/// </summary>
public class DependencyInjectionTests
{
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddEfCorePersistence<TestDbContext>(options =>
        {
            options.ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;";
            options.Provider = DatabaseProvider.SqlServer;
        });

        return services.BuildServiceProvider();
    }

    [Fact]
    public void Should_RegisterIReadRepository()
    {
        using var provider = BuildServiceProvider();

        var descriptor = provider.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider;

        var registration = new ServiceCollection();
        registration.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var readRepoDescriptor = registration.FirstOrDefault(
            d => d.ServiceType == typeof(IReadRepository<,>));

        readRepoDescriptor.Should().NotBeNull();
        readRepoDescriptor!.ImplementationType.Should().Be(typeof(EfReadRepository<,>));
        readRepoDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterIWriteRepository()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IWriteRepository<,>));

        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(EfWriteRepository<,>));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterIRepository()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IRepository<,>));

        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(EfRepository<,>));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterIAggregateRepository()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IAggregateRepository<,>));

        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(EfAggregateRepository<,>));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterIProjectionReadRepository()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IProjectionReadRepository<,>));

        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(EfProjectionReadRepository<,>));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterIUnitOfWork()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IUnitOfWork));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterITransactionManager()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(ITransactionManager));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_RegisterDbContextAsBaseType()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(DbContext));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AllPersistenceServices_Should_BeScoped()
    {
        var services = new ServiceCollection();
        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var persistenceTypes = new[]
        {
            typeof(IReadRepository<,>),
            typeof(IWriteRepository<,>),
            typeof(IRepository<,>),
            typeof(IAggregateRepository<,>),
            typeof(IProjectionReadRepository<,>),
            typeof(IUnitOfWork),
            typeof(ITransactionManager)
        };

        foreach (var type in persistenceTypes)
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == type);
            descriptor.Should().NotBeNull($"{type.Name} should be registered");
            descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped,
                $"{type.Name} should be registered as Scoped");
        }
    }

    [Fact]
    public void Should_ThrowWhenConnectionStringIsEmpty()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "";
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ConnectionString*");
    }

    [Fact]
    public void Should_ThrowWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Action act = () => services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;";
        });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_ThrowWhenConfigureIsNull()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddEfCorePersistence<TestDbContext>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}

/// <summary>
/// PTST-032: Integration tests for DI lifetime behavior.
/// </summary>
public class DiLifetimeTests
{
    [Fact]
    public void Should_RegisterCorrectNumberOfServices()
    {
        var services = new ServiceCollection();
        var countBefore = services.Count;

        services.AddEfCorePersistence<TestDbContext>(o =>
        {
            o.ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;";
        });

        var addedCount = services.Count - countBefore;

        // Should register: TDbContext, DbContext, 5 repos, UoW, TransactionManager = 9+
        addedCount.Should().BeGreaterThanOrEqualTo(9);
    }
}

/// <summary>
/// PTST-034/035: Integration tests for database provider configuration.
/// </summary>
public class DatabaseProviderConfigurationTests
{
    [Fact]
    public void PersistenceOptions_DefaultProvider_Should_BeSqlServer()
    {
        var options = new PersistenceOptions();

        options.Provider.Should().Be(DatabaseProvider.SqlServer);
    }

    [Fact]
    public void PersistenceOptions_Should_SupportPostgreSql()
    {
        var options = new PersistenceOptions
        {
            Provider = DatabaseProvider.PostgreSql
        };

        options.Provider.Should().Be(DatabaseProvider.PostgreSql);
    }

    [Fact]
    public void PersistenceOptions_Should_SupportMigrationAssembly()
    {
        var options = new PersistenceOptions
        {
            MigrationAssembly = "MyApp.Infrastructure"
        };

        options.MigrationAssembly.Should().Be("MyApp.Infrastructure");
    }

    [Fact]
    public void PersistenceOptions_Should_SupportInterceptors()
    {
        var options = new PersistenceOptions();
        var interceptor = new Moq.Mock<Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor>();

        options.AddInterceptor(interceptor.Object);

        options.Interceptors.Should().HaveCount(1);
    }

    [Fact]
    public void PersistenceOptions_AddInterceptor_Should_ThrowOnNull()
    {
        var options = new PersistenceOptions();

        Action act = () => options.AddInterceptor(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PersistenceOptions_Should_SupportFluentInterceptorChaining()
    {
        var options = new PersistenceOptions();
        var interceptor1 = new Moq.Mock<Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor>();
        var interceptor2 = new Moq.Mock<Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor>();

        var result = options.AddInterceptor(interceptor1.Object).AddInterceptor(interceptor2.Object);

        result.Should().BeSameAs(options);
        options.Interceptors.Should().HaveCount(2);
    }

    [Fact]
    public void PersistenceOptions_HealthCheck_DefaultDisabled()
    {
        var options = new PersistenceOptions();

        options.EnableHealthCheck.Should().BeFalse();
    }

    [Fact]
    public void PersistenceOptions_HealthCheckName_ShouldDefaultToPersistence()
    {
        var options = new PersistenceOptions();

        options.HealthCheckName.Should().Be("persistence");
    }
}
